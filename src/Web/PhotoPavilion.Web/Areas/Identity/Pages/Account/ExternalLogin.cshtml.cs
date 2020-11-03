namespace PhotoPavilion.Web.Areas.Identity.Pages.Account
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Text;
    using System.Text.Encodings.Web;
    using System.Threading.Tasks;

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
    public class ExternalLoginModel : PageModel
#pragma warning restore SA1649 // File name should match first type name
    {
        private readonly SignInManager<PhotoPavilionUser> signInManager;
        private readonly UserManager<PhotoPavilionUser> userManager;
        private readonly IEmailSender emailSender;
        private readonly IShoppingCartsService shoppingCartsService;
        private readonly ILogger<ExternalLoginModel> logger;

        public ExternalLoginModel(
            SignInManager<PhotoPavilionUser> signInManager,
            UserManager<PhotoPavilionUser> userManager,
            ILogger<ExternalLoginModel> logger,
            IEmailSender emailSender,
            IShoppingCartsService shoppingCartsService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.logger = logger;
            this.emailSender = emailSender;
            this.shoppingCartsService = shoppingCartsService;
        }

        [BindProperty]
        public ExternalLoginInputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public IActionResult OnGetAsync()
        {
            return this.RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = this.Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = this.signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl ??= this.Url.Content("~/");
            if (remoteError != null)
            {
                this.ErrorMessage = $"Error from external provider: {remoteError}";
                return this.RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await this.signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                this.ErrorMessage = "Error loading external login information.";
                return this.RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await this.signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                this.logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return this.LocalRedirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                return this.RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                this.ReturnUrl = returnUrl;
                this.LoginProvider = info.LoginProvider;
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    this.Input = new ExternalLoginInputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        FullName = info.Principal.FindFirstValue(ClaimTypes.Name),
                    };
                }

                return this.Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? this.Url.Content("~/");

            // Get the information about the user from the external login provider
            var info = await this.signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                this.ErrorMessage = "Error loading external login information during confirmation.";
                return this.RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (this.ModelState.IsValid)
            {
                Enum.TryParse<Gender>(this.Input.SelectedGender, out Gender gender);

                var user = new PhotoPavilionUser
                {
                    UserName = this.Input.Username,
                    Email = this.Input.Email,
                    Gender = gender,
                    FullName = this.Input.FullName,
                    ShoppingCart = new ShoppingCart(),
                };

                var result = await this.userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await this.userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        this.logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        await this.userManager.AddToRoleAsync(user, GlobalConstants.UserRoleName);
                        await this.shoppingCartsService.AssignShoppingCartToUserIdAsync(user);
                        await this.StoreGuestShoppingCartIfAny(info.Principal.Identity.Name.RemoveWhiteSpaces());

                        // If account confirmation is required, we need to show the link if we don't have a real email sender
                        if (this.userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return this.RedirectToPage("./RegisterConfirmation", new { Email = this.Input.Email });
                        }

                        await this.signInManager.SignInAsync(user, isPersistent: false);
                        var userId = await this.userManager.GetUserIdAsync(user);
                        var code = await this.userManager.GenerateEmailConfirmationTokenAsync(user);
                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = this.Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code },
                            protocol: this.Request.Scheme);

                        await this.emailSender.SendEmailAsync(
                            this.Input.Email,
                            "Confirm your email",
                            $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                        return this.LocalRedirect(returnUrl);
                    }
                }

                foreach (var error in result.Errors)
                {
                    this.ModelState.AddModelError(string.Empty, error.Description);
                }

                return this.Page();
            }

            this.LoginProvider = info.LoginProvider;
            this.ReturnUrl = returnUrl;
            return this.Page();
        }

        private async Task StoreGuestShoppingCartIfAny(string identityName)
        {
            var shoppingCartProducts = this.HttpContext.Session
                .GetObjectFromJson<ShoppingCartProductViewModel[]>(WebConstants.ShoppingCartSessionKey) ??
                new List<ShoppingCartProductViewModel>().ToArray();

            if (shoppingCartProducts != null)
            {
                foreach (var product in shoppingCartProducts)
                {
                    await this.shoppingCartsService.AddProductToShoppingCartAsync(product.ProductId, identityName, product.Quantity);
                }

                this.HttpContext.Session.Remove(WebConstants.ShoppingCartSessionKey);
            }
        }
    }
}
