namespace PhotoPavilion.Services.Data.Tests
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Models.Enumerations;
    using PhotoPavilion.Data.Repositories;
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
        private Product firstProduct;
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
        }

        private async void SeedDatabase()
        {
            await this.SeedBrands();
            await this.SeedCategories();
            await this.SeedShoppingCarts();
            await this.SeedUsers();
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
