namespace PhotoPavilion.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Products;
    using PhotoPavilion.Models.ViewModels;
    using PhotoPavilion.Models.ViewModels.Brands;
    using PhotoPavilion.Models.ViewModels.Categories;
    using PhotoPavilion.Models.ViewModels.Products;
    using PhotoPavilion.Services.Data.Contracts;

    public class ProductsController : AdministrationController
    {
        private const int PageSize = 10;
        private readonly IProductsService productsService;
        private readonly ICategoriesService categoriesService;
        private readonly IBrandsService brandsService;

        public ProductsController(
            IProductsService productsService,
            ICategoriesService categoriesService,
            IBrandsService brandsService)
        {
            this.productsService = productsService;
            this.categoriesService = categoriesService;
            this.brandsService = brandsService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public async Task<IActionResult> Create()
        {
            var categories = await this.categoriesService
                .GetAllCategoriesAsync<CategoryDetailsViewModel>();
            var brands = await this.brandsService
                .GetAllBrandsAsync<BrandDetailsViewModel>();
            var model = new ProductCreateInputModel
            {
                Categories = categories,
                Brands = brands,
            };

            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateInputModel productCreateInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                var categories = await this.categoriesService
                    .GetAllCategoriesAsync<CategoryDetailsViewModel>();
                var brands = await this.brandsService
                    .GetAllBrandsAsync<BrandDetailsViewModel>();

                productCreateInputModel.Categories = categories;
                productCreateInputModel.Brands = brands;

                return this.View(productCreateInputModel);
            }

            await this.productsService.CreateAsync(productCreateInputModel);
            return this.RedirectToAction("GetAll", "Products", new { area = "Administration" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var categories = await this.categoriesService
                .GetAllCategoriesAsync<CategoryDetailsViewModel>();
            var brands = await this.brandsService
                .GetAllBrandsAsync<BrandDetailsViewModel>();

            var productToEdit = await this.productsService
                .GetViewModelByIdAsync<ProductEditViewModel>(id);

            productToEdit.Categories = categories;
            productToEdit.Brands = brands;

            return this.View(productToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProductEditViewModel productEditViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                var categories = await this.categoriesService
                    .GetAllCategoriesAsync<CategoryDetailsViewModel>();
                var brands = await this.brandsService
                    .GetAllBrandsAsync<BrandDetailsViewModel>();

                productEditViewModel.Categories = categories;
                productEditViewModel.Brands = brands;

                return this.View(productEditViewModel);
            }

            await this.productsService.EditAsync(productEditViewModel);
            return this.RedirectToAction("GetAll", "Products", new { area = "Administration" });
        }

        public async Task<IActionResult> Remove(int id)
        {
            var productToDelete = await this.productsService.GetViewModelByIdAsync<ProductDeleteViewModel>(id);
            return this.View(productToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(ProductDeleteViewModel productDeleteViewModel)
        {
            await this.productsService.DeleteByIdAsync(productDeleteViewModel.Id);
            return this.RedirectToAction("GetAll", "Products", new { area = "Administration" });
        }

        public async Task<IActionResult> GetAll(int? pageNumber)
        {
            var products = await Task.Run(() =>
                this.productsService.GetAllProductsAsQueryeable<ProductDetailsViewModel>());
            return this.View(await PaginatedList<ProductDetailsViewModel>.CreateAsync(products, pageNumber ?? 1, PageSize));
        }
    }
}
