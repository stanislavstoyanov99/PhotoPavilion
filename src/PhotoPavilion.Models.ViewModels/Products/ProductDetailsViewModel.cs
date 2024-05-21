namespace PhotoPavilion.Models.ViewModels.Products
{
    using System;
    using Ganss.Xss;
    using System.Linq;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Models.ViewModels.ProductComments;
    using PhotoPavilion.Models.ViewModels.Brands;
    using PhotoPavilion.Models.ViewModels.Categories;
    using PhotoPavilion.Services.Mapping;

    using AutoMapper;

    using static PhotoPavilion.Models.Common.ModelValidation;
    using Product = Data.Models.Product;
    using PhotoPavilion.Models.ViewModels.Reviews;

    public class ProductDetailsViewModel : IMapFrom<Product>, IHaveCustomMappings
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
                        ? shortDescription[..200] + " ..."
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

        public int StarRatingsSum { get; set; }

        public IEnumerable<PostProductCommentViewModel> Comments { get; set; }

        public IEnumerable<PostProductReviewViewModel> Reviews { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Product, ProductDetailsViewModel>()
                .ForMember(x => x.StarRatingsSum, options =>
                {
                    options.MapFrom(p => p.Ratings.Sum(st => st.Rate));
                });
        }
    }
}
