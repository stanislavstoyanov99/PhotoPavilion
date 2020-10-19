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
    using PhotoPavilion.Models.InputModels.ProductComments;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;

    using Xunit;

    public class ProductCommentsServiceTests : IDisposable
    {
        private const string TestImagePath = "https://someimageurl.com";

        private readonly IProductCommentsService productCommentsService;
        private EfDeletableEntityRepository<Comment> productCommentsRepository;
        private EfDeletableEntityRepository<Product> productsRepository;
        private EfDeletableEntityRepository<Brand> brandsRepository;
        private EfDeletableEntityRepository<Category> categoriesRepository;
        private EfDeletableEntityRepository<PhotoPavilionUser> usersRepository;
        private EfDeletableEntityRepository<ShoppingCart> shoppingCartsRepository;
        private SqliteConnection connection;

        private Product firstProduct;
        private Comment firstProductComment;
        private Brand firstBrand;
        private Category firstCategory;
        private PhotoPavilionUser user;
        private ShoppingCart firstShoppingCart;

        public ProductCommentsServiceTests()
        {
            this.InitializeMapper();
            this.InitializeDatabaseAndRepositories();
            this.InitializeFields();

            this.productCommentsService = new ProductCommentsService(this.productCommentsRepository);
        }

        [Fact]
        public async Task TestAddingProductComment()
        {
            this.SeedDatabase();

            var productComment = new CreateProductCommentInputModel
            {
                ProductId = this.firstProduct.Id,
                Content = "I like this product.",
            };

            await this.productCommentsService.CreateAsync(productComment.ProductId, this.user.Id, productComment.Content);
            var count = await this.productCommentsRepository.All().CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckSettingOfProductCommentProperties()
        {
            this.SeedDatabase();

            var model = new CreateProductCommentInputModel
            {
                ProductId = this.firstProduct.Id,
                Content = "What's your opinion for the product?",
            };

            await this.productCommentsService.CreateAsync(model.ProductId, this.user.Id, model.Content);

            var productComment = await this.productCommentsRepository.All().FirstOrDefaultAsync();

            Assert.Equal(model.ProductId, productComment.ProductId);
            Assert.Equal("What's your opinion for the product?", productComment.Content);
        }

        [Fact]
        public async Task CheckIfAddingProductCommentThrowsArgumentException()
        {
            this.SeedDatabase();
            await this.SeedProductComments();

            var productComment = new CreateProductCommentInputModel
            {
                ProductId = this.firstProduct.Id,
                Content = this.firstProductComment.Content,
            };

            var exception = await Assert
                .ThrowsAsync<ArgumentException>(async ()
                    => await this.productCommentsService
                    .CreateAsync(productComment.ProductId, this.user.Id, productComment.Content));

            Assert.Equal(
                string.Format(
                    ExceptionMessages.ProductCommentAlreadyExists, productComment.ProductId, productComment.Content), exception.Message);
        }

        [Fact]
        public async Task CheckIfIsInProductIdReturnsTrue()
        {
            this.SeedDatabase();
            await this.SeedProductComments();

            var productCommentId = await this.productCommentsRepository
                .All()
                .Select(x => x.ProductId)
                .FirstOrDefaultAsync();

            var result = await this.productCommentsService.IsInProductId(productCommentId, this.firstProduct.Id);

            Assert.True(result);
        }

        [Fact]
        public async Task CheckIfIsInProductIdReturnsFalse()
        {
            this.SeedDatabase();
            await this.SeedProductComments();

            var result = await this.productCommentsService.IsInProductId(3, this.firstProduct.Id);

            Assert.False(result);
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
            this.shoppingCartsRepository = new EfDeletableEntityRepository<ShoppingCart>(dbContext);
            this.productsRepository = new EfDeletableEntityRepository<Product>(dbContext);
            this.brandsRepository = new EfDeletableEntityRepository<Brand>(dbContext);
            this.categoriesRepository = new EfDeletableEntityRepository<Category>(dbContext);
            this.productCommentsRepository = new EfDeletableEntityRepository<Comment>(dbContext);
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
                Name = "Nikon",
            };

            this.firstCategory = new Category
            {
                Name = "Cameras",
                Description = "Category only for cameras",
            };

            this.firstProduct = new Product
            {
                Id = 1,
                Name = "Nikon 7200D",
                Code = 10200,
                Description = "Sample description for Nikon 7200D",
                Price = 3500,
                ImagePath = TestImagePath,
                BrandId = 1,
                CategoryId = 1,
            };

            this.firstProductComment = new Comment
            {
                ProductId = this.firstProduct.Id,
                Content = "Test comment here",
                UserId = this.user.Id,
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

        private async Task SeedProductComments()
        {
            await this.productCommentsRepository.AddAsync(this.firstProductComment);

            await this.productCommentsRepository.SaveChangesAsync();
        }

        private void InitializeMapper() => AutoMapperConfig.
            RegisterMappings(Assembly.Load("PhotoPavilion.Models.ViewModels"));
    }
}
