namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PhotoPavilion.Models.ViewModels.OrderProducts;
    using PhotoPavilion.Models.ViewModels.ShoppingCart;

    public interface IOrderProductsService
    {
        Task<OrderProductDetailsViewModel> GetDetailsAsync(int id);

        Task<IEnumerable<OrderProductDetailsViewModel>> GetAllAsync(string username);

        Task BuyAllAsync(string userIdentifier, ShoppingCartProductViewModel[] shoppingCartActivities, string paymentMethod = "");
    }
}
