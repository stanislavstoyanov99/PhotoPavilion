namespace PhotoPavilion.Models.ViewModels.Products
{
    using System.Collections.Generic;

    public class ProductsHomePageListingViewModel
    {
        public IEnumerable<TopProductDetailsViewModel> TopProducts { get; set; }

        // public IEnumerable<SliderMovieDetailsViewModel> TopProductsInSlider { get; set; }
    }
}
