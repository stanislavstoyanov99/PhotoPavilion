namespace PhotoPavilion.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Common.Models;

    using static PhotoPavilion.Data.Common.DataValidation.Review;

    public class Review : BaseDeletableModel<int>
    {
        public Review()
        {
            this.ProductReviews = new HashSet<ProductReview>();
        }

        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; }

        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        public string UserId { get; set; }

        public virtual PhotoPavilionUser User { get; set; }

        public virtual ICollection<ProductReview> ProductReviews { get; set; }
    }
}
