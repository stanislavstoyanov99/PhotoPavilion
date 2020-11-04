namespace PhotoPavilion.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Models.Enumerations;
    using PhotoPavilion.Data.Repositories;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;

    using Xunit;

    public class ShoppingCartsServiceTests : IDisposable
    {
        private const string TestProductImageUrl = "https://someproductimageurl.com";

        private readonly IShoppingCartsService shoppingCartsService;

        private EfDeletableEntityRepository<Product> productsRepository;
        private EfDeletableEntityRepository<Brand> brandsRepository;
        private EfDeletableEntityRepository<Category> categoriesRepository;
        private EfDeletableEntityRepository<ShoppingCart> shoppingCartsRepository;
        private EfDeletableEntityRepository<ShoppingCartProduct> shoppingCartProductsRepository;
        private EfDeletableEntityRepository<PhotoPavilionUser> usersRepository;

        private SqliteConnection connection;

        private ShoppingCartProduct firstShoppingCartProduct;
        private Product firstProduct;
        private Brand firstBrand;
        private Category firstCategory;
        private PhotoPavilionUser user;

        public ShoppingCartsServiceTests()
        {
            this.InitializeMapper();
            this.InitializeDatabaseAndRepositories();
            this.InitializeFields();

            this.shoppingCartsService = new ShoppingCartsService(
                this.usersRepository,
                this.productsRepository,
                this.shoppingCartProductsRepository,
                this.shoppingCartsRepository);
        }

        [Fact]
        public async Task CheckIfAssignShoppingCartToUserIdAsyncThrowsNullReferenceException()
        {
            this.SeedDatabase();

            var shoppingCart = await this.shoppingCartsRepository.All().FirstOrDefaultAsync();
            this.shoppingCartsRepository.Delete(shoppingCart);
            await this.shoppingCartsRepository.SaveChangesAsync();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.shoppingCartsService.AssignShoppingCartToUserIdAsync(this.user));

            Assert.Equal(
                string.Format(ExceptionMessages.NullReferenceShoppingCart, this.user.Id, this.user.UserName), exception.Message);
        }

        [Fact]
        public async Task CheckIfAssignShoppingCartToUserIdAsyncWorksCorrectly()
        {
            this.SeedDatabase();

            await this.shoppingCartsService.AssignShoppingCartToUserIdAsync(this.user);

            var shoppingCart = await this.shoppingCartsRepository.All().FirstOrDefaultAsync();
            Assert.Equal("1", shoppingCart.UserId);
        }

        [Fact]
        public async Task CheckAddingProductToShoppingCartWithInvalidProduct()
        {
            this.SeedDatabase();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.shoppingCartsService.AddProductToShoppingCartAsync(2, this.user.UserName, 2));

            Assert.Equal(
                string.Format(ExceptionMessages.ProductNotFound, 2), exception.Message);
        }

        [Fact]
        public async Task CheckAddingProductToShoppingCartWithInvalidUsername()
        {
            this.SeedDatabase();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.shoppingCartsService.AddProductToShoppingCartAsync(1, "admin123", 2));

            Assert.Equal(
                string.Format(ExceptionMessages.NullReferenceUsername, "admin123"), exception.Message);
        }

        [Fact]
        public async Task CheckAddingProductToShoppingCartWithInvalidQuantity()
        {
            this.SeedDatabase();

            var exception = await Assert
                .ThrowsAsync<InvalidOperationException>(async () =>
                    await this.shoppingCartsService.AddProductToShoppingCartAsync(1, this.user.UserName, -2));

            Assert.Equal(
                string.Format(ExceptionMessages.ZeroOrNegativeQuantity), exception.Message);
        }

        [Fact]
        public async Task CheckIfAddProductToShoppingCartAsyncWorksCorrectly()
        {
            this.SeedDatabase();

            await this.shoppingCartsService.AddProductToShoppingCartAsync(1, this.user.UserName, 1);

            var count = await this.shoppingCartProductsRepository.All().CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckDeletingProductFromShoppingCartWithMissingShoppingCartProduct()
        {
            this.SeedDatabase();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.shoppingCartsService.DeleteProductFromShoppingCartAsync(2, this.user.UserName));

            Assert.Equal(
                string.Format(ExceptionMessages.NullReferenceShoppingCartProductId, 2), exception.Message);
        }

        [Fact]
        public async Task CheckDeletingProductFromShoppingCartWithMissingUsername()
        {
            this.SeedDatabase();
            await this.SeedShoppingCartProducts();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.shoppingCartsService.DeleteProductFromShoppingCartAsync(1, "admin123"));

            Assert.Equal(
                string.Format(ExceptionMessages.NullReferenceUsername, "admin123"), exception.Message);
        }

        [Fact]
        public async Task CheckIfDeleteProductFromShoppingCartAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedShoppingCartProducts();

            await this.shoppingCartsService.DeleteProductFromShoppingCartAsync(1, this.user.UserName);

            var count = await this.shoppingCartProductsRepository.All().CountAsync();

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task CheckEditingShoppingCartProductWithMissingShoppingProductOrder()
        {
            this.SeedDatabase();
            await this.SeedShoppingCartProducts();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.shoppingCartsService.EditShoppingCartProductAsync(2, this.user.UserName, 2));

            Assert.Equal(
                string.Format(ExceptionMessages.NullReferenceShoppingCartProductId, 2), exception.Message);
        }

        [Fact]
        public async Task CheckEditingShoppingCartProductWithMissingUsername()
        {
            this.SeedDatabase();
            await this.SeedShoppingCartProducts();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.shoppingCartsService.EditShoppingCartProductAsync(1, "admin123", 2));

            Assert.Equal(
                string.Format(ExceptionMessages.NullReferenceUsername, "admin123"), exception.Message);
        }

        [Fact]
        public async Task CheckEditingShoppingCartProductWithInvalidQuantity()
        {
            this.SeedDatabase();
            await this.SeedShoppingCartProducts();

            var exception = await Assert
                .ThrowsAsync<InvalidOperationException>(async () =>
                    await this.shoppingCartsService.EditShoppingCartProductAsync(1, this.user.UserName, -3));

            Assert.Equal(
                string.Format(ExceptionMessages.ZeroOrNegativeQuantity, -3), exception.Message);
        }

        [Fact]
        public async Task CheckIfEditShoppingCartProductAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedShoppingCartProducts();

            await this.shoppingCartsService.EditShoppingCartProductAsync(1, this.user.UserName, 2);

            var shoppingProductOrder = await this.shoppingCartProductsRepository.All().FirstOrDefaultAsync();

            Assert.Equal(2, shoppingProductOrder.Quantity);
        }

        [Fact]
        public async Task CheckGettingAllShoppingCartProductsWithMissingUsername()
        {
            this.SeedDatabase();
            await this.SeedShoppingCartProducts();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.shoppingCartsService.GetAllShoppingCartProductsAsync("admin123"));

            Assert.Equal(
                string.Format(ExceptionMessages.NullReferenceUsername, "admin123"), exception.Message);
        }

        [Fact]
        public async Task CheckIfGetAllShoppingCartProductsAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedShoppingCartProducts();

            var shoppingCartProducts = await this.shoppingCartsService.GetAllShoppingCartProductsAsync(this.user.UserName);

            var count = shoppingCartProducts.Count();
            var firstShoppingCartProduct = shoppingCartProducts.First();

            Assert.Equal(1, count);
            Assert.Equal(this.firstShoppingCartProduct.Id, firstShoppingCartProduct.Id);
            Assert.Equal(this.firstShoppingCartProduct.ShoppingCart.User.UserName, this.user.UserName);
        }

        [Fact]
        public async Task CheckClearingShoppingCartWithMissingUsername()
        {
            this.SeedDatabase();
            await this.SeedShoppingCartProducts();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.shoppingCartsService.ClearShoppingCartAsync("admin123"));

            Assert.Equal(
                string.Format(ExceptionMessages.NullReferenceUsername, "admin123"), exception.Message);
        }

        [Fact]
        public async Task CheckIfClearShoppingCartAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedShoppingCartProducts();

            await this.shoppingCartsService.ClearShoppingCartAsync(this.user.UserName);

            var count = await this.shoppingCartProductsRepository.All().CountAsync();

            Assert.Equal(0, count);
        }

        public void Dispose()
        {
            this.connection.Close();
            this.connection.Dispose();
        }

        private void InitializeDatabaseAndRepositories()
        {
            this.connection = new SqliteConnection("DataSource=:memory:");
            this.connection.Open();
            var options = new DbContextOptionsBuilder<PhotoPavilionDbContext>().UseSqlite(this.connection);
            var dbContext = new PhotoPavilionDbContext(options.Options);

            dbContext.Database.EnsureCreated();

            this.usersRepository = new EfDeletableEntityRepository<PhotoPavilionUser>(dbContext);
            this.productsRepository = new EfDeletableEntityRepository<Product>(dbContext);
            this.brandsRepository = new EfDeletableEntityRepository<Brand>(dbContext);
            this.categoriesRepository = new EfDeletableEntityRepository<Category>(dbContext);
            this.shoppingCartsRepository = new EfDeletableEntityRepository<ShoppingCart>(dbContext);
            this.shoppingCartProductsRepository = new EfDeletableEntityRepository<ShoppingCartProduct>(dbContext);
        }

        private void InitializeFields()
        {
            this.user = new PhotoPavilionUser
            {
                Id = "1",
                Gender = Gender.Male,
                UserName = "stamat123",
                FullName = "Stamat Stamatov",
                Email = "test_email@gmail.com",
                PasswordHash = "123456",
                ShoppingCart = new ShoppingCart(),
            };

            this.firstBrand = new Brand
            {
                Name = "Nikon",
            };

            this.firstCategory = new Category
            {
                Name = "Cameras",
                Description = "Sample category description",
            };

            this.firstProduct = new Product
            {
                Id = 1,
                Name = "Nikon D7200",
                Code = 10200,
                Description = "Sample description for Nikon D7200",
                Price = 1300,
                ImagePath = TestProductImageUrl,
                BrandId = 1,
                CategoryId = 1,
            };

            this.firstShoppingCartProduct = new ShoppingCartProduct
            {
                ProductId = 1,
                ShoppingCartId = 1,
                Quantity = 1,
            };
        }

        private async void SeedDatabase()
        {
            await this.SeedBrands();
            await this.SeedCategories();
            await this.SeedUsers();
            await this.SeedProducts();
        }

        private async Task SeedUsers()
        {
            await this.usersRepository.AddAsync(this.user);

            await this.usersRepository.SaveChangesAsync();
        }

        private async Task SeedShoppingCartProducts()
        {
            await this.shoppingCartProductsRepository.AddAsync(this.firstShoppingCartProduct);

            await this.shoppingCartProductsRepository.SaveChangesAsync();
        }

        private async Task SeedProducts()
        {
            await this.productsRepository.AddAsync(this.firstProduct);

            await this.productsRepository.SaveChangesAsync();
        }

        private async Task SeedBrands()
        {
            await this.brandsRepository.AddAsync(this.firstBrand);

            await this.brandsRepository.SaveChangesAsync();
        }

        private async Task SeedCategories()
        {
            await this.categoriesRepository.AddAsync(this.firstCategory);

            await this.categoriesRepository.SaveChangesAsync();
        }

        private void InitializeMapper() => AutoMapperConfig.
            RegisterMappings(Assembly.Load("PhotoPavilion.Models.ViewModels"));
    }
}
