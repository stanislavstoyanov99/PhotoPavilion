namespace PhotoPavilion.Web.Areas.Identity.Pages.Account
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Identity.UI.Services;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using Microsoft.AspNetCore.WebUtilities;
    using Microsoft.Extensions.Logging;

    using PhotoPavilion.Common;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Models.Enumerations;
    using PhotoPavilion.Models.ViewModels.ShoppingCart;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Web.Areas.Identity.Pages.Account.InputModels;
    using PhotoPavilion.Web.Common;
    using PhotoPavilion.Web.Helpers;

    [AllowAnonymous]
#pragma warning disable SA1649 // File name should match first type name
    public class RegisterModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly SignInManager<PhotoPavilionUser> signInManager;
        private readonly UserManager<PhotoPavilionUser> userManager;
        private readonly ILogger<RegisterModel> logger;
        private readonly IEmailSender emailSender;
        private readonly IShoppingCartsService shoppingCartsService;

        public RegisterModel(
            UserManager<PhotoPavilionUser> userManager,
            SignInManager<PhotoPavilionUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            IShoppingCartsService shoppingCartsService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.shoppingCartsService = shoppingCartsService;
        }

        [BindProperty(SupportsGet = true)]
        public RegisterInputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (!this.User.Identity.IsAuthenticated)
            {
                this.ReturnUrl = returnUrl;
                this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                return this.Page();
            }
            else
            {
                return this.Redirect("/");
            }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= this.Url.Content("~/");
            this.ExternalLogins = (await this.signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (this.ModelState.IsValid)
            {
                Enum.TryParse<Gender>(this.Input.SelectedGender, out Gender gender);

                var user = new PhotoPavilionUser
                {
                    UserName = this.Input.Username,
                    Email = this.Input.Email,
                    FullName = this.Input.FullName,
                    Gender = gender,
                    ShoppingCart = new ShoppingCart(),
                };

                var result = await this.userManager.CreateAsync(user, this.Input.Password);
                if (result.Succeeded)
                {
                    this.logger.LogInformation("User created a new account with password.");
                    await this.userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);
                    await this.shoppingCartsService.AssignShoppingCartToUserId(user);

                    var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = this.Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = user.Id, code = code },
                        protocol: this.Request.Scheme);

                    await this.emailSender.SendEmailAsync(
                        this.Input.Email,
                        "Confirm your email",
                        htmlMessage: $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (this.userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return this.RedirectToPage("RegisterConfirmation", new { email = this.Input.Email });
                    }
                    else
                    {
                        await this.signInManager.SignInAsync(user, isPersistent: false);

                        var shoppingCartProducts = this.HttpContext.Session
                               .GetObjectFromJson<ShoppingCartProductViewModel[]>(WebConstants.ShoppingCartSessionKey) ??
                               new List<ShoppingCartProductViewModel>().ToArray();

                        if (shoppingCartProducts != null)
                        {
                            foreach (var product in shoppingCartProducts)
                            {
                                await this.shoppingCartsService
                                    .AddProductToShoppingCartAsync(product.ProductId, this.Input.Username, product.Quantity);
                            }

                            this.HttpContext.Session.Remove(WebConstants.ShoppingCartSessionKey);
                        }

                        return this.LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Page();
        }
    }
}
