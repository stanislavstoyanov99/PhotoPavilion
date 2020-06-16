namespace PhotoPavilion.Models.ViewModels.OrderProducts
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Models.Enumerations;
    using PhotoPavilion.Models.Common;
    using PhotoPavilion.Services.Mapping;

    using static PhotoPavilion.Models.Common.ModelValidation.OrderProduct;

    public class OrderProductDetailsViewModel : IMapFrom<OrderProduct>
    {
        [Display(Name = OrderIdDisplay)]
        public int Id { get; set; }

        public string UserId { get; set; }

        public PhotoPavilionUser User { get; set; }

        [Display(Name = UserFullNameDisplay)]
        public string UserFullName { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        [Display(Name = ModelValidation.Product.NameDisplay)]
        public string ProductName { get; set; }

        public int Quantity { get; set; }

        [Display(Name = OrderStatusNameDisplay)]
        public OrderStatus Status { get; set; }

        [Display(Name = CreatedOnNameDisplay)]
        public DateTime Date { get; set; }
    }
}
