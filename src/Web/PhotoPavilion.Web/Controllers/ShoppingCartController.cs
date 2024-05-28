namespace PhotoPavilion.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using PhotoPavilion.Common;
    using PhotoPavilion.Models.ViewModels.ShoppingCart;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Web.Common;

    using Stripe;

    [Authorize]
    public class ShoppingCartController : Controller
    {
        private readonly IShoppingCartsService shoppingCartService;
        private readonly IOrderProductsService orderProductsService;

        public ShoppingCartController(
            IShoppingCartsService shoppingCartService,
            IOrderProductsService orderProductsService)
        {
            this.shoppingCartService = shoppingCartService;
            this.orderProductsService = orderProductsService;
        }

        public async Task<IActionResult> Index()
        {
            var username = this.User.Identity.Name;

            var shoppingCart = await this.shoppingCartService.GetAllShoppingCartProductsAsync(username);

            return this.View(shoppingCart);
        }

        public async Task<IActionResult> Add(int id, int quantity)
        {
            var username = this.User.Identity.Name;
            await this.shoppingCartService.AddProductToShoppingCartAsync(id, username, quantity);

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id, int newQuantity)
        {
            var username = this.User.Identity.Name;
            await this.shoppingCartService.EditShoppingCartProductAsync(id, username, newQuantity);

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var username = this.User.Identity.Name;
            await this.shoppingCartService.DeleteProductFromShoppingCartAsync(id, username);

            return this.RedirectToAction("Index");
        }

        // Stripe
        public async Task<IActionResult> Charge(string stripeEmail, string stripeToken)
        {
            var userName = this.User.Identity.Name;
            var userProducts = await this.shoppingCartService.GetAllShoppingCartProductsAsync(userName);

            CreateCharge(stripeEmail, stripeToken, userName, userProducts);

            await this.orderProductsService
                .BuyAllAsync(userName, userProducts.ToArray(), GlobalConstants.OnlinePaymentMethod);
            this.HttpContext.Session.Remove(WebConstants.ShoppingCartSessionKey);

            return this.View("_BuyingConfirmation");
        }

        private static void CreateCharge(
            string stripeEmail,
            string stripeToken,
            string userName,
            IEnumerable<ShoppingCartProductViewModel> userProducts)
        {
            var customers = new CustomerService();
            var charges = new ChargeService();

            var customer = customers.Create(new CustomerCreateOptions
            {
                Email = stripeEmail,
                Source = stripeToken,
            });

            var totalSum = userProducts.Sum(up => up.ShoppingCartProductTotalPrice);
            var totalSumInCents = totalSum * 100;

            var productLabel = userProducts.Count() == 1 ? "product" : "products";

            charges.Create(new ChargeCreateOptions
            {
                Amount = (long)totalSumInCents,
                Description = $"{userName} bought {userProducts.Count()} {productLabel} on {DateTime.UtcNow}",
                Currency = "usd",
                Customer = customer.Id,
                ReceiptEmail = stripeEmail,
            });
        }
    }
}
