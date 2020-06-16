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
        private const int TopProductsCount = 12;
        private const int TopProductsRating = 30;

        private readonly IProductsService productsService;
        private readonly IPrivacyService privacyService;

        public HomeController(IProductsService productsService, IPrivacyService privacyService)
        {
            this.productsService = productsService;
            this.privacyService = privacyService;
        }

        public async Task<IActionResult> Index()
        {
            var topProducts = await this.productsService
                .GetTopRatingProductsAsync<TopProductDetailsViewModel>(TopProductsRating, TopProductsCount);

            var viewModel = new ProductsHomePageListingViewModel
            {
                TopProducts = topProducts,
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
