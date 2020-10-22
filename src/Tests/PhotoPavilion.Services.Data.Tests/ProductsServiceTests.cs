namespace PhotoPavilion.Services.Data.Tests
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    using CloudinaryDotNet;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Models.Enumerations;
    using PhotoPavilion.Data.Repositories;
    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Products;
    using PhotoPavilion.Models.ViewModels.Products;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;

    using Xunit;

    public class ProductsServiceTests : IDisposable, IClassFixture<Configuration>
    {
        private const string TestImagePath = "Test.jpg";
        private const string TestImageContentType = "image/jpg";
        private const string TestProductImageUrl = "https://someproductimageurl.com";

        private readonly IProductsService productsService;
        private readonly ICloudinaryService cloudinaryService;
        private readonly Cloudinary cloudinary;

        private EfDeletableEntityRepository<Product> productsRepository;
        private EfDeletableEntityRepository<Brand> brandsRepository;
        private EfDeletableEntityRepository<Category> categoriesRepository;
        private EfDeletableEntityRepository<ShoppingCart> shoppingCartsRepository;
        private EfDeletableEntityRepository<PhotoPavilionUser> usersRepository;
        private SqliteConnection connection;

        private ShoppingCart firstShoppingCart;
        private Product firstProduct;
        private Brand firstBrand;
        private Category firstCategory;
        private PhotoPavilionUser user;

        public ProductsServiceTests(Configuration configuration)
        {
            this.InitializeMapper();
            this.InitializeDatabaseAndRepositories();
            this.InitializeFields();

            Account account = new Account(
               configuration.ConfigurationRoot["Cloudinary:AppName"],
               configuration.ConfigurationRoot["Cloudinary:AppKey"],
               configuration.ConfigurationRoot["Cloudinary:AppSecret"]);

            this.cloudinary = new Cloudinary(account);
            this.cloudinaryService = new CloudinaryService(this.cloudinary);

            this.productsService = new ProductsService(
                this.productsRepository,
                this.brandsRepository,
                this.categoriesRepository,
                this.cloudinaryService);
        }

        [Fact]
        public async Task TestAddingProduct()
        {
            this.SeedDatabase();

            ProductDetailsViewModel productDetailsViewModel;

            using (var stream = File.OpenRead(TestImagePath))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = TestImageContentType,
                };

                var model = new ProductCreateInputModel
                {
                    Name = this.firstProduct.Name,
                    Code = this.firstProduct.Code,
                    Description = this.firstProduct.Description,
                    Price = this.firstProduct.Price,
                    Image = file,
                    BrandId = 1,
                    CategoryId = 1,
                };

                productDetailsViewModel = await this.productsService.CreateAsync(model);
            }

            await this.cloudinaryService.DeleteImage(this.cloudinary, productDetailsViewModel.Name + Suffixes.ProductSuffix);

            var count = await this.productsRepository.All().CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckIfAddingProductReturnsViewModel()
        {
            this.SeedDatabase();

            ProductDetailsViewModel productDetailsViewModel;

            using (var stream = File.OpenRead(TestImagePath))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = TestImageContentType,
                };

                var model = new ProductCreateInputModel
                {
                    Name = this.firstProduct.Name,
                    Code = this.firstProduct.Code,
                    Description = this.firstProduct.Description,
                    Price = this.firstProduct.Price,
                    Image = file,
                    BrandId = 1,
                    CategoryId = 1,
                };

                productDetailsViewModel = await this.productsService.CreateAsync(model);
            }

            await this.cloudinaryService.DeleteImage(this.cloudinary, productDetailsViewModel.Name + Suffixes.ProductSuffix);

            var product = await this.productsRepository.All().FirstOrDefaultAsync();

            Assert.Equal("Canon eos 1100D", product.Name);
            Assert.Equal(10600, product.Code);
            Assert.Equal("Sample description for Canon eos 1100D", product.Description);
            Assert.Equal(1500, product.Price);
            Assert.Equal(1, product.BrandId);
            Assert.Equal(1, product.CategoryId);
        }

        [Fact]
        public async Task TestAddingProductWithMissingBrand()
        {
            this.SeedDatabase();

            var model = new ProductCreateInputModel
            {
                Name = this.firstProduct.Name,
                Code = this.firstProduct.Code,
                Description = this.firstProduct.Description,
                Price = this.firstProduct.Price,
                BrandId = 3,
                CategoryId = 1,
            };

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () => await this.productsService.CreateAsync(model));
            Assert.Equal(string.Format(ExceptionMessages.BrandNotFound, model.BrandId), exception.Message);
        }

        [Fact]
        public async Task TestAddingProductWithMissingCategory()
        {
            this.SeedDatabase();

            var model = new ProductCreateInputModel
            {
                Name = this.firstProduct.Name,
                Code = this.firstProduct.Code,
                Description = this.firstProduct.Description,
                Price = this.firstProduct.Price,
                BrandId = 1,
                CategoryId = 3,
            };

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () => await this.productsService.CreateAsync(model));
            Assert.Equal(string.Format(ExceptionMessages.CategoryNotFound, model.CategoryId), exception.Message);
        }

        [Fact]
        public async Task TestAddingAlreadyExistingProduct()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            using (var stream = File.OpenRead(TestImagePath))
            {
                var file = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name))
                {
                    Headers = new HeaderDictionary(),
                    ContentType = TestImageContentType,
                };

                var model = new ProductCreateInputModel
                {
                    Name = this.firstProduct.Name,
                    Code = this.firstProduct.Code,
                    Description = this.firstProduct.Description,
                    Price = this.firstProduct.Price,
                    Image = file,
                    BrandId = 1,
                    CategoryId = 1,
                };

                var exception = await Assert
                    .ThrowsAsync<ArgumentException>(async () => await this.productsService.CreateAsync(model));

                await this.cloudinaryService.DeleteImage(this.cloudinary, model.Name + Suffixes.ProductSuffix);
                Assert.Equal(string.Format(ExceptionMessages.ProductAlreadyExists, model.Name), exception.Message);
            }
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
                UserName = "maria123",
                FullName = "Maria Petrova",
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
