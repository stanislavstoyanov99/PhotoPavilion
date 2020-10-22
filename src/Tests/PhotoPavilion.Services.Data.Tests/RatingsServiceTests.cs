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

    public class RatingsServiceTests : IDisposable
    {
        private readonly IRatingsService ratingsService;
        private EfDeletableEntityRepository<StarRating> starRatingsRepository;
        private EfDeletableEntityRepository<Product> productsRepository;
        private EfDeletableEntityRepository<Brand> brandsRepository;
        private EfDeletableEntityRepository<Category> categoriesRepository;
        private EfDeletableEntityRepository<ShoppingCart> shoppingCartsRepository;
        private EfDeletableEntityRepository<PhotoPavilionUser> usersRepository;
        private SqliteConnection connection;

        private StarRating firstStarRating;
        private ShoppingCart firstShoppingCart;
        private Product firstProduct;
        private Brand firstBrand;
        private Category firstCategory;
        private PhotoPavilionUser user;

        public RatingsServiceTests()
        {
            this.InitializeMapper();
            this.InitializeDatabaseAndRepositories();
            this.InitializeFields();

            this.ratingsService = new RatingsService(this.starRatingsRepository);
        }

        [Fact]
        public async Task CheckIfVoteAsyncWorksCorrectly()
        {
            this.SeedDatabase();

            await this.ratingsService.VoteAsync(this.firstProduct.Id, this.user.Id, 5);
            var count = await this.starRatingsRepository.All().CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckIfVoteAsyncThrowsArgumentException()
        {
            this.SeedDatabase();
            await this.SeedStarRatings();

            var exception = await Assert
                .ThrowsAsync<ArgumentException>(async () => await this.ratingsService.VoteAsync(this.firstProduct.Id, this.user.Id, 6));
            Assert.Equal(ExceptionMessages.AlreadySentVote, exception.Message);
        }

        [Fact]
        public async Task CheckIfVoteAsyncWorksCorrectlyAfterSecondVoting()
        {
            this.SeedDatabase();
            var starRating = new StarRating
            {
                Rate = 10,
                ProductId = 1,
                UserId = this.user.Id,
                NextVoteDate = DateTime.UtcNow.AddDays(-1),
            };
            await this.starRatingsRepository.AddAsync(starRating);
            await this.starRatingsRepository.SaveChangesAsync();

            await this.ratingsService.VoteAsync(this.firstProduct.Id, this.user.Id, 5);
            var count = await this.starRatingsRepository.All().CountAsync();
            var currentStarRating = await this.starRatingsRepository.All().FirstOrDefaultAsync();

            Assert.Equal(1, count);
            Assert.Equal(15, currentStarRating.Rate);
            Assert.Equal(starRating.NextVoteDate, currentStarRating.NextVoteDate);
        }

        [Fact]
        public async Task CheckIfGetStarRatingsAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedStarRatings();

            var result = await this.ratingsService.GetStarRatingsAsync(this.firstProduct.Id);

            Assert.Equal(5, result);
        }

        [Fact]
        public async Task CheckIfGetNextVoteDateAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedStarRatings();

            var result = await this.ratingsService.GetNextVoteDateAsync(this.firstProduct.Id, this.user.Id);

            Assert.Equal(this.firstStarRating.NextVoteDate, result);
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
            this.starRatingsRepository = new EfDeletableEntityRepository<StarRating>(dbContext);
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
                Description = "Sample category description",
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

            this.firstStarRating = new StarRating
            {
                Rate = 5,
                ProductId = 1,
                UserId = this.user.Id,
                NextVoteDate = DateTime.UtcNow.AddDays(1),
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

        private async Task SeedStarRatings()
        {
            await this.starRatingsRepository.AddAsync(this.firstStarRating);

            await this.starRatingsRepository.SaveChangesAsync();
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
