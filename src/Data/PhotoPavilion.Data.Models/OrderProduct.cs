namespace PhotoPavilion.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Common.Models;
    using PhotoPavilion.Data.Models.Enumerations;

    public class OrderProduct : BaseDeletableModel<int>
    {
        public string UserId { get; set; }

        public virtual PhotoPavilionUser User { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
