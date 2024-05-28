namespace PhotoPavilion.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.ViewModels.ShoppingCart;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;

    public class ShoppingCartsService : IShoppingCartsService
    {
        private readonly IDeletableEntityRepository<PhotoPavilionUser> usersRepository;
        private readonly IDeletableEntityRepository<Product> productsRepository;
        private readonly IDeletableEntityRepository<ShoppingCartProduct> shoppingCartProductsRepository;
        private readonly IDeletableEntityRepository<ShoppingCart> shoppingCartsRepository;

        public ShoppingCartsService(
            IDeletableEntityRepository<PhotoPavilionUser> usersRepository,
            IDeletableEntityRepository<Product> productsRepository,
            IDeletableEntityRepository<ShoppingCartProduct> shoppingCartProductsRepository,
            IDeletableEntityRepository<ShoppingCart> shoppingCartsRepository)
        {
            this.usersRepository = usersRepository;
            this.productsRepository = productsRepository;
            this.shoppingCartProductsRepository = shoppingCartProductsRepository;
            this.shoppingCartsRepository = shoppingCartsRepository;
        }

        public async Task AssignShoppingCartToUserIdAsync(PhotoPavilionUser user)
        {
            var shoppingCart = await this.shoppingCartsRepository
                .All()
                .FirstOrDefaultAsync(sc => sc.User.Id == user.Id);

            if (shoppingCart == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.NullReferenceShoppingCart, user.Id, user.UserName));
            }

            shoppingCart.UserId = user.Id;
            this.shoppingCartsRepository.Update(shoppingCart);
            await this.shoppingCartProductsRepository.SaveChangesAsync();
        }

        public async Task AddProductToShoppingCartAsync(int productId, string username, int quantity)
        {
            var product = await this.productsRepository
                .All()
                .FirstOrDefaultAsync(p => p.Id == productId);
            if (product == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.ProductNotFound, productId));
            }

            var user = await this.usersRepository
                .All()
                .FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.NullReferenceUsername, username));
            }

            if (quantity <= 0)
            {
                throw new InvalidOperationException(ExceptionMessages.ZeroOrNegativeQuantity);
            }

            var shoppingCartProduct = new ShoppingCartProduct
            {
                Product = product,
                ShoppingCartId = user.ShoppingCartId,
                Quantity = quantity,
            };

            await this.shoppingCartProductsRepository.AddAsync(shoppingCartProduct);
            await this.shoppingCartProductsRepository.SaveChangesAsync();
        }

        public async Task DeleteProductFromShoppingCartAsync(int shoppingCartProductId, string username)
        {
            var shoppingCartProduct = await this.shoppingCartProductsRepository
                .All()
                .FirstOrDefaultAsync(scp => scp.Id == shoppingCartProductId);
            if (shoppingCartProduct == null)
            {
                throw new NullReferenceException(string.Format(
                    ExceptionMessages.NullReferenceShoppingCartProductId, shoppingCartProductId));
            }

            var user = await this.usersRepository
                .All()
                .FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.NullReferenceUsername, username));
            }

            this.shoppingCartProductsRepository.Delete(shoppingCartProduct);
            await this.shoppingCartProductsRepository.SaveChangesAsync();
        }

        public async Task EditShoppingCartProductAsync(int shoppingCartProductId, string username, int newQuantity)
        {
            var shoppingProductOrder = await this.shoppingCartProductsRepository
                .All()
                .FirstOrDefaultAsync(scp => scp.Id == shoppingCartProductId);
            if (shoppingProductOrder == null)
            {
                throw new NullReferenceException(string.Format(
                    ExceptionMessages.NullReferenceShoppingCartProductId, shoppingCartProductId));
            }

            var user = await this.usersRepository
                .All()
                .FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.NullReferenceUsername, username));
            }

            if (newQuantity <= 0)
            {
                throw new InvalidOperationException(ExceptionMessages.ZeroOrNegativeQuantity);
            }

            shoppingProductOrder.Quantity = newQuantity;
            this.shoppingCartProductsRepository.Update(shoppingProductOrder);
            await this.shoppingCartProductsRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShoppingCartProductViewModel>> GetAllShoppingCartProductsAsync(string username)
        {
            var user = await this.usersRepository
                .All()
                .FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.NullReferenceUsername, username));
            }

            var shoppingCartProducts = await this.shoppingCartProductsRepository
                .All()
                .Where(scp => scp.ShoppingCart.User.UserName == username)
                .To<ShoppingCartProductViewModel>()
                .ToArrayAsync();

            return shoppingCartProducts;
        }

        public async Task ClearShoppingCartAsync(string username)
        {
            var user = await this.usersRepository
                .All()
                .FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.NullReferenceUsername, username));
            }

            var shoppingCartProducts = this.shoppingCartProductsRepository
                .All()
                .Where(scp => scp.ShoppingCart.User == user)
                .ToList();

            foreach (var shoppingCartProduct in shoppingCartProducts)
            {
                shoppingCartProduct.IsDeleted = true;
                this.shoppingCartProductsRepository.Update(shoppingCartProduct);
            }

            await this.shoppingCartProductsRepository.SaveChangesAsync();
        }
    }
}
