namespace PhotoPavilion.Models.ViewModels.Products
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Services.Mapping;
    using PhotoPavilion.Common.Attributes;
    using PhotoPavilion.Models.ViewModels.Brands;
    using PhotoPavilion.Models.ViewModels.Categories;

    using Microsoft.AspNetCore.Http;

    using static PhotoPavilion.Models.Common.ModelValidation;
    using static PhotoPavilion.Models.Common.ModelValidation.Product;
    using static PhotoPavilion.Models.Common.ModelValidation.Brand;
    using static PhotoPavilion.Models.Common.ModelValidation.Category;

    using Product = Data.Models.Product;
    using Brand = Data.Models.Brand;
    using Category = Data.Models.Category;

    public class ProductEditViewModel : IMapFrom<Product>, IMapFrom<Brand>, IMapFrom<Category>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = EmptyFieldLengthError)]
        [StringLength(NameMaxLength, MinimumLength = NameMinLength, ErrorMessage = NameLengthError)]
        public string Name { get; set; }

        [Range(1, CodeMaxLength)]
        public int Code { get; set; }

        [Required(ErrorMessage = EmptyFieldLengthError)]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = DescriptionError)]
        public string Description { get; set; }

        [Range(1, double.MaxValue)]
        public decimal Price { get; set; }

        [DataType(DataType.Url)]
        [StringLength(ImageMaxLength, MinimumLength = ImageMinLength, ErrorMessage = ImagePathError)]
        public string ImagePath { get; set; }

        [Required(ErrorMessage = EmptyFieldLengthError)]
        [DataType(DataType.Upload)]
        [MaxFileSize(ImageMaxSize)]
        [AllowedExtensions]
        public IFormFile Image { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = BrandIdError)]
        [Display(Name = nameof(Brand))]
        public int BrandId { get; set; }

        public IEnumerable<BrandDetailsViewModel> Brands { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = CategoryIdError)]
        [Display(Name = nameof(Category))]
        public int CategoryId { get; set; }

        public IEnumerable<CategoryDetailsViewModel> Categories { get; set; }
    }
}
