namespace PhotoPavilion.Web.Components
{
    using Microsoft.AspNetCore.Mvc;
    using PhotoPavilion.Services.Data.Contracts;

    public class ShoppingCartComponent : ViewComponent
    {
        private readonly IShoppingCartsService shoppingCartsService;

        public ShoppingCartComponent(IShoppingCartsService shoppingCartsService)
        {
            this.shoppingCartsService = shoppingCartsService;
        }

        public IViewComponentResult Invoke()
        {
            var username = this.User.Identity.Name;

            var shoppingCartProducts = this.shoppingCartsService
                .GetAllShoppingCartProductsAsync(username)
                .GetAwaiter()
                .GetResult();

            return this.View(shoppingCartProducts);
        }
    }
}
