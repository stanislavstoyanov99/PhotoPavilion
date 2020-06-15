namespace PhotoPavilion.Models.ViewModels.ShoppingCart
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Services.Mapping;

    using static PhotoPavilion.Models.Common.ModelValidation.ShoppingCartProduct;

    public class ShoppingCartProductViewModel : IMapFrom<ShoppingCartProduct>
    {
        public int Id { get; set; }

        public string ShoppingCartUserId { get; set; }

        public int ProductId { get; set; }

        public string ProductImagePath { get; set; }

        [Display(Name = ProductNameDisplayName)]
        public string ProductName { get; set; }

        [Display(Name = ProductCreatedOnDisplayName)]
        public DateTime ProductCreatedOn { get; set; }

        [Display(Name = ProductPriceDisplayName)]
        public decimal? ProductPrice { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public decimal? ShoppingCartProductTotalPrice => this.ProductPrice * this.Quantity;
    }
}
