namespace PhotoPavilion.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Common.Models;
    using PhotoPavilion.Data.Models.Enumerations;

    public class Order : BaseDeletableModel<int>
    {
        [Required]
        public OrderStatus Status { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public DateTime RequiredDate { get; set; }

        [Required]
        public DateTime ShippingDate { get; set; }

        [Required]
        public string UserId { get; set; }

        public virtual PhotoPavilionUser User { get; set; }
    }
}
