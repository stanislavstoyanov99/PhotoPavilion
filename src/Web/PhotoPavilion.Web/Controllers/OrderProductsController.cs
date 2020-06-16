namespace PhotoPavilion.Web.Controllers
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    using PhotoPavilion.Common;
    using PhotoPavilion.Models.ViewModels;
    using PhotoPavilion.Models.ViewModels.OrderProducts;
    using PhotoPavilion.Services.Data.Contracts;

    [Authorize]
    public class OrderProductsController : Controller
    {
        private const int PageSize = 12;

        private readonly IOrderProductsService orderProductsService;
        private readonly IShoppingCartsService shoppingCartsService;

        public OrderProductsController(IOrderProductsService orderProductsService, IShoppingCartsService shoppingCartsService)
        {
            this.orderProductsService = orderProductsService;
            this.shoppingCartsService = shoppingCartsService;
        }

        [HttpPost]
        public async Task<IActionResult> Buy()
        {
            var userName = this.User.Identity.Name;
            var shoppingCartProducts = await this.shoppingCartsService.GetAllShoppingCartProductsAsync(userName);

            await this.orderProductsService.BuyAllAsync(userName, shoppingCartProducts.ToArray(), GlobalConstants.CashPaymentMethod);
            return this.View("_BuyingConfirmation");
        }

        public async Task<IActionResult> Details(int id)
        {
            OrderProductDetailsViewModel orderProductDetailsViewModel;

            try
            {
                orderProductDetailsViewModel = await this.orderProductsService.GetDetailsAsync(id);

                if (orderProductDetailsViewModel.User == null || orderProductDetailsViewModel.User.UserName != this.User.Identity.Name)
                {
                    return this.View("_AccessDenied");
                }
            }
            catch (NullReferenceException)
            {
                return this.View("_AccessDenied");
            }

            return this.View(orderProductDetailsViewModel);
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            var username = this.User.Identity.Name;
            var orderProducts = await Task.Run(() =>
                this.orderProductsService.GetAllAsQueryeable(username));

            return this.View(await PaginatedList<OrderProductDetailsViewModel>.CreateAsync(orderProducts, pageNumber ?? 1, PageSize));
        }
    }
}
