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
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;

    using Xunit;

    public class ProductReviewsServiceTests : IDisposable
    {
        private readonly IProductReviewsService productReviewsService;
        private EfDeletableEntityRepository<Review> reviewsRepository;
        private EfDeletableEntityRepository<Product> productsRepository;
        private EfDeletableEntityRepository<Brand> brandsRepository;
        private EfDeletableEntityRepository<Category> categoriesRepository;
        private EfDeletableEntityRepository<ShoppingCart> shoppingCartsRepository;
        private EfDeletableEntityRepository<PhotoPavilionUser> usersRepository;
        private SqliteConnection connection;

        private Review firstReview;
        private Product firstProduct;
        private Brand firstBrand;
        private Category firstCategory;
        private PhotoPavilionUser user;
        private ShoppingCart firstShoppingCart;

        public ProductReviewsServiceTests()
        {
            this.InitializeMapper();
            this.InitializeDatabaseAndRepositories();
            this.InitializeFields();

            this.productReviewsService = new ProductReviewsService(this.reviewsRepository);
        }

        [Fact]
        public async Task TestAddingProductReview()
        {
            this.SeedDatabase();

            await this.productReviewsService.CreateAsync(this.firstProduct.Id, this.user.Id, "Sample title", "Sample description");
            var count = await this.reviewsRepository.All().CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckIfAddingProductReviewThrowsArgumentException()
        {
            this.SeedDatabase();
            await this.SeedReviews();

            var exception = await Assert
                .ThrowsAsync<ArgumentException>(async () => await this.productReviewsService
                    .CreateAsync(this.firstProduct.Id, this.user.Id, this.firstReview.Title, this.firstReview.Description));
            Assert.Equal(
                string.Format(
                ExceptionMessages.ProductReviewAlreadyExists,
                this.firstReview.Id,
                this.firstReview.Title,
                this.firstReview.Description), exception.Message);
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
            this.reviewsRepository = new EfDeletableEntityRepository<Review>(dbContext);
            this.productsRepository = new EfDeletableEntityRepository<Product>(dbContext);
            this.brandsRepository = new EfDeletableEntityRepository<Brand>(dbContext);
            this.categoriesRepository = new EfDeletableEntityRepository<Category>(dbContext);
            this.shoppingCartsRepository = new EfDeletableEntityRepository<ShoppingCart>(dbContext);
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
                Gender = Gender.Male,
                UserName = "pesho123",
                FullName = "Pesho Peshov",
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
                Description = "Samle category description",
            };

            this.firstProduct = new Product
            {
                Id = 1,
                Name = "Canon eos 1100D",
                Code = 10600,
                Description = "Sample description for Canon eos 1100D",
                Price = 1500,
                ImagePath = "test image path",
                BrandId = 1,
                CategoryId = 1,
            };

            this.firstReview = new Review
            {
                Title = "Nice product to buy.",
                Description = "Sample description about this product.",
                UserId = "1",
                ProductId = 1,
            };
        }

        private async void SeedDatabase()
        {
            await this.SeedBrands();
            await this.SeedCategories();
            await this.SeedProducts();
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

        private async Task SeedReviews()
        {
            await this.reviewsRepository.AddAsync(this.firstReview);

            await this.reviewsRepository.SaveChangesAsync();
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
