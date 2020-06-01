namespace PhotoPavilion.Models.InputModels.AdministratorInputModels.Products
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Http;

    using PhotoPavilion.Common.Attributes;
    using PhotoPavilion.Models.ViewModels.Brands;
    using PhotoPavilion.Models.ViewModels.Categories;

    using static PhotoPavilion.Models.Common.ModelValidation;
    using static PhotoPavilion.Models.Common.ModelValidation.Product;
    using static PhotoPavilion.Models.Common.ModelValidation.Brand;
    using static PhotoPavilion.Models.Common.ModelValidation.Category;

    using Brand = Data.Models.Brand;
    using Category = Data.Models.Category;

    public class ProductCreateInputModel
    {
        [Required(ErrorMessage = EmptyFieldLengthError)]
        [StringLength(Common.ModelValidation.Category.NameMaxLength, MinimumLength = Common.ModelValidation.Category.NameMinLength, ErrorMessage = NameLengthError)]
        public string Name { get; set; }

        [Range(1, CodeMaxLength)]
        public int Code { get; set; }

        [Required(ErrorMessage = EmptyFieldLengthError)]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = DescriptionError)]
        public string Description { get; set; }

        [Range(1, 20000)]
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
