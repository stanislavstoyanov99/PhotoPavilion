namespace PhotoPavilion.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PhotoPavilion.Models.ViewModels;
    using PhotoPavilion.Models.ViewModels.Categories;
    using PhotoPavilion.Models.ViewModels.Products;
    using PhotoPavilion.Services.Data.Contracts;

    public class CategoriesController : Controller
    {
        private const int ProductsPerPage = 6;
        private readonly IProductsService productsService;
        private readonly ICategoriesService categoriesService;

        public CategoriesController(IProductsService productsService, ICategoriesService categoriesService)
        {
            this.productsService = productsService;
            this.categoriesService = categoriesService;
        }

        public async Task<IActionResult> ByName(int? pageNumber, string name)
        {
            var productsByCategoryName = await Task.Run(() =>
                this.productsService.GetByCategoryNameAsQueryable(name));

            if (productsByCategoryName.Count() == 0)
            {
                return this.NotFound();
            }

            this.TempData["CategoryName"] = name;

            var productsByCategoryNamePaginated = await PaginatedList<ProductDetailsViewModel>
                    .CreateAsync(productsByCategoryName, pageNumber ?? 1, ProductsPerPage);

            var lastlyAddedProduct = await this.productsService
                .GetLastlyAddedProductAsync<ProductDetailsViewModel>();

            var category = await this.categoriesService
                .GetCategoryAsync<CategoryDetailsViewModel>(name);

            var viewModel = new ProductCategoryPageListingViewModel
            {
                Category = category,
                ProductsByCategoryName = productsByCategoryNamePaginated,
                LastlyAddedProduct = lastlyAddedProduct,
            };

            return this.View(viewModel);
        }
    }
}
