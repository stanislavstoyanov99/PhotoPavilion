namespace PhotoPavilion.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PhotoPavilion.Models.ViewModels.Products;
    using PhotoPavilion.Services.Data.Contracts;

    public class ProductsController : Controller
    {
        private readonly IProductsService productsService;

        public ProductsController(IProductsService productsService)
        {
            this.productsService = productsService;
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await this.productsService.GetViewModelByIdAsync<ProductDetailsViewModel>(id);

            var viewModel = new DetailsListingViewModel
            {
                ProductDetailsViewModel = product,
            };

            return this.View(viewModel);
        }
    }
}
