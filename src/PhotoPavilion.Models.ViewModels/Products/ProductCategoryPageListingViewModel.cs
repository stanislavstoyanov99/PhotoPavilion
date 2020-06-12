namespace PhotoPavilion.Models.ViewModels.Products
{
    using PhotoPavilion.Models.ViewModels.Categories;

    public class ProductCategoryPageListingViewModel
    {
        public PaginatedList<ProductDetailsViewModel> ProductsByCategoryName { get; set; }

        public ProductDetailsViewModel LastlyAddedProduct { get; set; }

        public CategoryDetailsViewModel Category { get; set; }
    }
}
