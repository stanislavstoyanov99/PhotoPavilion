namespace PhotoPavilion.Data.Models
{
    using System;

    using PhotoPavilion.Data.Common.Models;

    public class OrderProduct : IDeletableEntity
    {
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedOn { get; set; }
    }
}
