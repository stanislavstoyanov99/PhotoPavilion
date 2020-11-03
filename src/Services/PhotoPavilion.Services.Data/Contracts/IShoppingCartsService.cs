namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.ViewModels.ShoppingCart;

    public interface IShoppingCartsService
    {
        Task AssignShoppingCartToUserIdAsync(PhotoPavilionUser user);

        Task<IEnumerable<ShoppingCartProductViewModel>> GetAllShoppingCartProductsAsync(string username);

        Task AddProductToShoppingCartAsync(int productId, string username, int quantity);

        Task EditShoppingCartProductAsync(int shoppingCartProductId, string username, int newQuantity);

        Task DeleteProductFromShoppingCart(int shoppingCartProductId, string username);

        Task ClearShoppingCart(string username);
    }
}
