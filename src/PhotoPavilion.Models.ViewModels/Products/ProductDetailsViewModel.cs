namespace PhotoPavilion.Models.ViewModels.Products
{
    using System;
    using Ganss.XSS;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Models.ViewModels.Brands;
    using PhotoPavilion.Models.ViewModels.Categories;
    using PhotoPavilion.Services.Mapping;

    using static PhotoPavilion.Models.Common.ModelValidation;
    using Product = Data.Models.Product;

    public class ProductDetailsViewModel : IMapFrom<Product>
    {
        [Display(Name = IdDisplayName)]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Code { get; set; }

        public string Description { get; set; }

        public string ShortDescription
        {
            get
            {
                var shortDescription = this.Description;
                return shortDescription.Length > 200
                        ? shortDescription.Substring(0, 200) + " ..."
                        : shortDescription;
            }
        }

        public string SanitizedShortDescription => new HtmlSanitizer().Sanitize(this.ShortDescription);

        public decimal Price { get; set; }

        [Display(Name = nameof(Brand))]
        public BrandDetailsViewModel Brand { get; set; }

        [Display(Name = nameof(Category))]
        public CategoryDetailsViewModel Category { get; set; }

        public string ImagePath { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
