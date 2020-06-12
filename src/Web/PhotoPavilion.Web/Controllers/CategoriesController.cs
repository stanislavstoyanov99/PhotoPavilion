namespace PhotoPavilion.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PhotoPavilion.Models.ViewModels;
    using PhotoPavilion.Models.ViewModels.Products;
    using PhotoPavilion.Services.Data.Contracts;

    public class CategoriesController : Controller
    {
        private const int ProductsPerPage = 6;
        private readonly IProductsService productsService;

        public CategoriesController(IProductsService productsService)
        {
            this.productsService = productsService;
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

            return this.View(productsByCategoryNamePaginated);
        }
    }
}
