namespace PhotoPavilion.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Models.Enumerations;
    using PhotoPavilion.Data.Repositories;
    using PhotoPavilion.Models.ViewModels.OrderProducts;
    using PhotoPavilion.Models.ViewModels.ShoppingCart;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;
    using PhotoPavilion.Services.Messaging;

    using Xunit;

    public class OrderProductsServiceTests : IDisposable, IClassFixture<Configuration>
    {
        private const string TestProductImageUrl = "https://someproductimageurl.com";

        private readonly IOrderProductsService orderProductsService;
        private readonly IShoppingCartsService shoppingCartsService;
        private readonly IEmailSender emailSender;

        private EfDeletableEntityRepository<Product> productsRepository;
        private EfDeletableEntityRepository<Brand> brandsRepository;
        private EfDeletableEntityRepository<Category> categoriesRepository;
        private EfDeletableEntityRepository<ShoppingCart> shoppingCartsRepository;
        private EfDeletableEntityRepository<ShoppingCartProduct> shoppingCartProductsRepository;
        private EfDeletableEntityRepository<PhotoPavilionUser> usersRepository;
        private EfDeletableEntityRepository<OrderProduct> orderProductsRepository;

        private SqliteConnection connection;

        private ShoppingCart firstShoppingCart;
        private ShoppingCartProduct firstShoppingCartProduct;
        private Product firstProduct;
        private OrderProduct firstOrderProduct;
        private Brand firstBrand;
        private Category firstCategory;
        private PhotoPavilionUser user;

        public OrderProductsServiceTests(Configuration configuration)
        {
            this.InitializeMapper();
            this.InitializeDatabaseAndRepositories();
            this.InitializeFields();

            this.shoppingCartsService = new ShoppingCartsService(
                this.usersRepository,
                this.productsRepository,
                this.shoppingCartProductsRepository,
                this.shoppingCartsRepository);

            this.emailSender = new SendGridEmailSender(configuration.ConfigurationRoot["SendGrid:ApiKey"]);
            this.orderProductsService = new OrderProductsService(
                this.orderProductsRepository,
                this.usersRepository,
                this.productsRepository,
                this.shoppingCartsService,
                this.emailSender);
        }

        [Fact]
        public async Task CheckIfGetAllOrderProductsAsQueryeableWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedOrderProducts();

            var result = this.orderProductsService.GetAllAsQueryeable("peter123");

            var count = await result.CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckIfGetDetailsAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedOrderProducts();

            var orderProduct = await this.orderProductsService.GetDetailsAsync(this.firstOrderProduct.Id);

            Assert.Equal(this.firstOrderProduct.UserId, orderProduct.UserId);
            Assert.Equal(this.firstOrderProduct.User.FullName, orderProduct.UserFullName);
            Assert.Equal(this.firstOrderProduct.ProductId, orderProduct.ProductId);
            Assert.Equal(this.firstOrderProduct.Product.Name, orderProduct.ProductName);
            Assert.Equal(this.firstOrderProduct.Quantity, orderProduct.Quantity);
            Assert.Equal(this.firstOrderProduct.Status, orderProduct.Status);
            Assert.Equal(this.firstOrderProduct.Date, orderProduct.Date);
        }

        [Fact]
        public async Task CheckIfGetDetailsAsyncThrowsNullReferenceException()
        {
            this.SeedDatabase();
            await this.SeedOrderProducts();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.orderProductsService.GetDetailsAsync(3));
            Assert.Equal(string.Format(ExceptionMessages.OrderProductNotFound, 3), exception.Message);
        }

        [Fact]
        public async Task CheckIfBuyUsersTicketsAsyncThrowsNullReferenceExceptionWithMissingUser()
        {
            this.SeedDatabase();
            await this.SeedOrderProducts();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.orderProductsService.BuyAllAsync("pesho123", null, "cash"));
            Assert.Equal(string.Format(ExceptionMessages.NullReferenceUsername, "pesho123"), exception.Message);
        }

        [Fact]
        public async Task CheckIfBuyUsersTicketsAsyncThrowsNullReferenceExceptionWithInvalidQuantity()
        {
            this.SeedDatabase();
            await this.SeedOrderProducts();

            var secondShoppingCartProduct = new ShoppingCartProduct
            {
                ShoppingCartId = 1,
                ProductId = 1,
                Quantity = -2,
            };
            await this.shoppingCartProductsRepository.AddAsync(secondShoppingCartProduct);
            await this.shoppingCartProductsRepository.SaveChangesAsync();

            var shoppingCartProducts = await this.shoppingCartsService.GetAllShoppingCartProductsAsync("peter123");

            var exception = await Assert
                .ThrowsAsync<InvalidOperationException>(async () =>
                    await this.orderProductsService.BuyAllAsync("peter123", shoppingCartProducts.ToArray(), "cash"));
            Assert.Equal(string.Format(ExceptionMessages.ZeroOrNegativeQuantity), exception.Message);
        }

        [Fact]
        public async Task CheckIfBuyUsersTicketsAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedShoppingCartProducts();

            var shoppingCartProducts = await this.shoppingCartsService.GetAllShoppingCartProductsAsync("peter123");

            await this.orderProductsService.BuyAllAsync("peter123", shoppingCartProducts.ToArray(), "cash");

            var count = await this.orderProductsRepository.All().CountAsync();
            var firstOrderProduct = await this.orderProductsRepository.All().FirstOrDefaultAsync();

            Assert.Equal(1, count);
            Assert.Equal(1, firstOrderProduct.Id);
            Assert.Equal("1", firstOrderProduct.UserId);
            Assert.Equal(1, firstOrderProduct.ProductId);
            Assert.Equal(1, firstOrderProduct.Quantity);
            Assert.Equal(OrderStatus.Pending, firstOrderProduct.Status);
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

            this.orderProductsRepository = new EfDeletableEntityRepository<OrderProduct>(dbContext);
            this.usersRepository = new EfDeletableEntityRepository<PhotoPavilionUser>(dbContext);
            this.productsRepository = new EfDeletableEntityRepository<Product>(dbContext);
            this.brandsRepository = new EfDeletableEntityRepository<Brand>(dbContext);
            this.categoriesRepository = new EfDeletableEntityRepository<Category>(dbContext);
            this.shoppingCartsRepository = new EfDeletableEntityRepository<ShoppingCart>(dbContext);
            this.shoppingCartProductsRepository = new EfDeletableEntityRepository<ShoppingCartProduct>(dbContext);
        }

        private void InitializeFields()
        {
            this.firstShoppingCart = new ShoppingCart
            {
                UserId = "1",
            };

            this.user = new PhotoPavilionUser
            {
                Id = "1",
                Gender = Gender.Female,
                UserName = "peter123",
                FullName = "Peter Petrov",
                Email = "test_email@gmail.com",
                PasswordHash = "123456",
                ShoppingCartId = 1,
            };

            this.firstBrand = new Brand
            {
                Name = "Canon",
            };

            this.firstCategory = new Category
            {
                Name = "Cameras",
                Description = "Sample category description",
            };

            this.firstProduct = new Product
            {
                Id = 1,
                Name = "Canon eos 1100D",
                Code = 10600,
                Description = "Sample description for Canon eos 1100D",
                Price = 1500,
                ImagePath = TestProductImageUrl,
                BrandId = 1,
                CategoryId = 1,
            };

            this.firstOrderProduct = new OrderProduct
            {
                ProductId = 1,
                Quantity = 1,
                Status = OrderStatus.Accepted,
                Date = DateTime.UtcNow,
                UserId = "1",
            };

            this.firstShoppingCartProduct = new ShoppingCartProduct
            {
                ShoppingCartId = 1,
                ProductId = 1,
                Quantity = 1,
            };
        }

        private async void SeedDatabase()
        {
            await this.SeedBrands();
            await this.SeedCategories();
            await this.SeedShoppingCarts();
            await this.SeedUsers();
            await this.SeedProducts();
        }

        private async Task SeedUsers()
        {
            await this.usersRepository.AddAsync(this.user);

            await this.usersRepository.SaveChangesAsync();
        }

        private async Task SeedShoppingCarts()
        {
            await this.shoppingCartsRepository.AddAsync(this.firstShoppingCart);

            await this.shoppingCartsRepository.SaveChangesAsync();
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

        private async Task SeedOrderProducts()
        {
            await this.orderProductsRepository.AddAsync(this.firstOrderProduct);

            await this.orderProductsRepository.SaveChangesAsync();
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
