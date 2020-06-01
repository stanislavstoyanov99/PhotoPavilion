namespace PhotoPavilion.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Brands;
    using PhotoPavilion.Models.ViewModels;
    using PhotoPavilion.Models.ViewModels.Brands;
    using PhotoPavilion.Services.Data.Contracts;

    public class BrandsController : AdministrationController
    {
        private const int PageSize = 10;

        private readonly IBrandsService brandsService;

        public BrandsController(IBrandsService brandsService)
        {
            this.brandsService = brandsService;
        }

        public IActionResult Index()
        {
            return this.View();
        }

        public IActionResult Create()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BrandCreateInputModel brandCreateInputModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(brandCreateInputModel);
            }

            await this.brandsService.CreateAsync(brandCreateInputModel);
            return this.RedirectToAction("GetAll", "Brands", new { area = "Administration" });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var brandToEdit = await this.brandsService
                .GetViewModelByIdAsync<BrandEditViewModel>(id);

            return this.View(brandToEdit);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BrandEditViewModel brandEditViewModel)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(brandEditViewModel);
            }

            await this.brandsService.EditAsync(brandEditViewModel);
            return this.RedirectToAction("GetAll", "Brands", new { area = "Administration" });
        }

        public async Task<IActionResult> Remove(int id)
        {
            var categoryToDelete = await this.brandsService.GetViewModelByIdAsync<BrandDetailsViewModel>(id);
            return this.View(categoryToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Remove(BrandDetailsViewModel brandViewModel)
        {
            await this.brandsService.DeleteByIdAsync(brandViewModel.Id);
            return this.RedirectToAction("GetAll", "Brands", new { area = "Administration" });
        }

        public async Task<IActionResult> GetAll(int? pageNumber)
        {
            var products = await Task.Run(() =>
                this.brandsService.GetAllBrandsAsQueryeable<BrandDetailsViewModel>());
            return this.View(await PaginatedList<BrandDetailsViewModel>.CreateAsync(products, pageNumber ?? 1, PageSize));
        }
    }
}
