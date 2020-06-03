namespace PhotoPavilion.Models.ViewModels.Products
{
    using System.Collections.Generic;

    public class ProductsHomePageListingViewModel
    {
        public IEnumerable<TopProductDetailsViewModel> TopCameras { get; set; }

        public IEnumerable<TopProductDetailsViewModel> TopLenses { get; set; }

        public IEnumerable<TopProductDetailsViewModel> TopFilters { get; set; }

        public IEnumerable<TopProductDetailsViewModel> TopLightenings { get; set; }
    }
}
