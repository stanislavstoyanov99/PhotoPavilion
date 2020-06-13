namespace PhotoPavilion.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Common.Models;

    using static PhotoPavilion.Data.Common.DataValidation.Product;

    public class Product : BaseDeletableModel<int>
    {
        public Product()
        {
            this.ProductReviews = new HashSet<ProductReview>();
            this.Comments = new HashSet<Comment>();
            this.Ratings = new HashSet<StarRating>();
        }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        public int Code { get; set; }

        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        public decimal Price { get; set; }

        [Required]
        [MaxLength(ImagePathMaxLength)]
        public string ImagePath { get; set; }

        public int BrandId { get; set; }

        public virtual Brand Brand { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<ProductReview> ProductReviews { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<StarRating> Ratings { get; set; }
    }
}
