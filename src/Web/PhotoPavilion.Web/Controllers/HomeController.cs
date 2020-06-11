namespace PhotoPavilion.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using PhotoPavilion.Models.ViewModels;
    using PhotoPavilion.Models.ViewModels.Privacy;
    using PhotoPavilion.Models.ViewModels.Products;
    using PhotoPavilion.Services.Data.Contracts;

    public class HomeController : Controller
    {
        private const int TopCamerasCount = 6;
        private const int TopLensesCount = 3;
        private const int TopFiltersCount = 3;
        private const int TopLighteningsCount = 3;
        private const string CamerasCategory = "Cameras";
        private const string LensesCategory = "Lenses";
        private const string FiltersCategory = "Filters";
        private const string LighteningsCategory = "Lightenings";

        private readonly IProductsService productsService;
        private readonly IPrivacyService privacyService;

        public HomeController(IProductsService productsService, IPrivacyService privacyService)
        {
            this.productsService = productsService;
            this.privacyService = privacyService;
        }

        public async Task<IActionResult> Index()
        {
            var topCameras = await this.productsService
                .GetTopProductsAsync<TopProductDetailsViewModel>(CamerasCategory, TopCamerasCount);
            var topLenses = await this.productsService
                .GetTopProductsAsync<TopProductDetailsViewModel>(LensesCategory, TopLensesCount);
            var topFilters = await this.productsService
                .GetTopProductsAsync<TopProductDetailsViewModel>(FiltersCategory, TopFiltersCount);
            var topLightenings = await this.productsService
                .GetTopProductsAsync<TopProductDetailsViewModel>(LighteningsCategory, TopLighteningsCount);

            var viewModel = new ProductsHomePageListingViewModel
            {
                TopCameras = topCameras,
                TopLenses = topLenses,
                TopFilters = topFilters,
                TopLightenings = topLightenings,
            };

            return this.View(viewModel);
        }

        public async Task<IActionResult> Privacy()
        {
            var privacy = await this.privacyService.GetViewModelAsync<PrivacyDetailsViewModel>();

            return this.View(privacy);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error(HttpErrorViewModel errorViewModel)
        {
            if (errorViewModel.StatusCode == 404)
            {
                return this.View(
                "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
            }

            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        public IActionResult HttpError(HttpErrorViewModel errorViewModel)
        {
            if (errorViewModel.StatusCode == 404)
            {
                return this.View(errorViewModel);
            }

            return this.View(
                "Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
