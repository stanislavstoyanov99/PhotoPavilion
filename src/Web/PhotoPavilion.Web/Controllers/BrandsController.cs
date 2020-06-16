namespace PhotoPavilion.Web.Controllers
{
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;
    using PhotoPavilion.Models.ViewModels.Brands;
    using PhotoPavilion.Services.Data.Contracts;

    public class BrandsController : Controller
    {
        private readonly IBrandsService brandsService;

        public BrandsController(IBrandsService brandsService)
        {
            this.brandsService = brandsService;
        }

        public async Task<IActionResult> Index()
        {
            var allBrands = await this.brandsService
                .GetAllBrandsAsync<BrandDetailsViewModel>();

            return this.View(allBrands);
        }
    }
}
