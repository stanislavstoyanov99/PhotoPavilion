namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Linq;
    using System.Threading.Tasks;

    using PhotoPavilion.Models.ViewModels.OrderProducts;
    using PhotoPavilion.Models.ViewModels.ShoppingCart;

    public interface IOrderProductsService
    {
        Task<OrderProductDetailsViewModel> GetDetailsAsync(int id);

        IQueryable<OrderProductDetailsViewModel> GetAllAsQueryeable(string username);

        Task BuyAllAsync(string userIdentifier, ShoppingCartProductViewModel[] shoppingCartActivities, string paymentMethod = "");
    }
}
