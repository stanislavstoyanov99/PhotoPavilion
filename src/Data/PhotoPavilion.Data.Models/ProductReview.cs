namespace PhotoPavilion.Data.Models
{
    using System;

    using PhotoPavilion.Data.Common.Models;

    public class ProductReview : IDeletableEntity
    {
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int? ReviewId { get; set; }

        public virtual Review Review { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
