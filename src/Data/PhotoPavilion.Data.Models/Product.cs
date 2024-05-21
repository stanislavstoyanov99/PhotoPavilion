namespace PhotoPavilion.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using Microsoft.EntityFrameworkCore;
    using PhotoPavilion.Data.Common.Models;

    using static PhotoPavilion.Data.Common.DataValidation.Product;

    public class Product : BaseDeletableModel<int>
    {
        public Product()
        {
            this.Reviews = new HashSet<Review>();
            this.Comments = new HashSet<Comment>();
            this.Ratings = new HashSet<StarRating>();
            this.ShoppingCartProducts = new HashSet<ShoppingCartProduct>();
            this.OrderProducts = new HashSet<OrderProduct>();
        }

        [Required]
        [MaxLength(NameMaxLength)]
        public string Name { get; set; }

        public int Code { get; set; }

        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        [Precision(18, 2)]
        public decimal Price { get; set; }

        [Required]
        [MaxLength(ImagePathMaxLength)]
        public string ImagePath { get; set; }

        public int BrandId { get; set; }

        public virtual Brand Brand { get; set; }

        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        public virtual ICollection<StarRating> Ratings { get; set; }

        public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }
    }
}
