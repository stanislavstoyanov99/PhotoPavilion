namespace PhotoPavilion.Models.ViewModels.Products
{
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.ViewModels.Brands;
    using PhotoPavilion.Models.ViewModels.Categories;
    using PhotoPavilion.Services.Mapping;

    using static PhotoPavilion.Models.Common.ModelValidation.Product;

    public class ProductDeleteViewModel : IMapFrom<Product>, IMapFrom<Category>, IMapFrom<Brand>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        [Display(Name = ImageDisplayName)]
        public string ImagePath { get; set; }

        [Display(Name = nameof(Category))]
        public CategoryDetailsViewModel Category { get; set; }

        [Display(Name = nameof(Brand))]
        public BrandDetailsViewModel Brand { get; set; }
    }
}
