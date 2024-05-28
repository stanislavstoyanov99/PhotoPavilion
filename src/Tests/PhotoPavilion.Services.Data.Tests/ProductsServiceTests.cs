namespace PhotoPavilion.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using Newtonsoft.Json;

    using PhotoPavilion.Data;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Models.Enumerations;
    using PhotoPavilion.Data.Repositories;
    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Products;
    using PhotoPavilion.Models.ViewModels.Brands;
    using PhotoPavilion.Models.ViewModels.Categories;
    using PhotoPavilion.Models.ViewModels.ProductComments;
    using PhotoPavilion.Models.ViewModels.Products;
    using PhotoPavilion.Models.ViewModels.Reviews;
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

        [Fact]
        public async Task CheckIfDeletingProductWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            await this.productsService.DeleteByIdAsync(this.firstProduct.Id);

            var count = await this.productsRepository.All().CountAsync();

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task CheckIfDeletingProductReturnsNullReferenceException()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () => await this.productsService.DeleteByIdAsync(3));
            Assert.Equal(string.Format(ExceptionMessages.ProductNotFound, 3), exception.Message);
        }

        [Fact]
        public async Task EditAsyncEditsProductWhenImageStaysTheSame()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            var secondBrand = new Brand
            {
                Name = "Nikon",
            };
            await this.brandsRepository.AddAsync(secondBrand);
            await this.brandsRepository.SaveChangesAsync();

            var secondCategory = new Category
            {
                Name = "Lenses",
                Description = "Sample description for lenses",
            };
            await this.categoriesRepository.AddAsync(secondCategory);
            await this.categoriesRepository.SaveChangesAsync();

            var productEditViewModel = new ProductEditViewModel
            {
                Id = this.firstProduct.Id,
                Name = "Changed product name",
                Code = 10500,
                Description = "Changed product description",
                Price = 2000,
                BrandId = 2,
                CategoryId = 2,
                Image = null,
            };

            await this.productsService.EditAsync(productEditViewModel);

            Assert.Equal(productEditViewModel.Name, this.firstProduct.Name);
            Assert.Equal(productEditViewModel.Code, this.firstProduct.Code);
            Assert.Equal(productEditViewModel.Description, this.firstProduct.Description);
            Assert.Equal(productEditViewModel.Price, this.firstProduct.Price);
            Assert.Equal(productEditViewModel.BrandId, this.firstProduct.BrandId);
            Assert.Equal(productEditViewModel.CategoryId, this.firstProduct.CategoryId);
        }

        [Fact]
        public async Task CheckIfEditingProductReturnsNullReferenceException()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            var productEditViewModel = new ProductEditViewModel
            {
                Id = 3,
            };

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () => await this.productsService.EditAsync(productEditViewModel));
            Assert.Equal(string.Format(ExceptionMessages.ProductNotFound, productEditViewModel.Id), exception.Message);
        }

        [Fact]
        public async Task CheckIfEditAsyncEditsProductImage()
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

                var productEditViewModel = new ProductEditViewModel()
                {
                    Id = this.firstProduct.Id,
                    Name = "Changed product name",
                    Code = 10500,
                    Description = "Changed product description",
                    Price = 2000,
                    BrandId = 1,
                    CategoryId = 1,
                    Image = file,
                };

                await this.productsService.EditAsync(productEditViewModel);

                await this.cloudinaryService.DeleteImage(this.cloudinary, productEditViewModel.Name + Suffixes.ProductSuffix);
            }

            var product = await this.productsRepository.All().FirstAsync();
            Assert.NotEqual(TestProductImageUrl, product.ImagePath);
        }

        [Fact]
        public async Task CheckIfGetAllProductsAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            var result = await this.productsService.GetAllProductsAsync<ProductDetailsViewModel>();

            var count = result.Count();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckIfGetAllProductsAsQueryeableWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            var secondProduct = new Product
            {
                Name = "Alfa X Pro",
                Code = 10300,
                Description = "Sample description for Alfa X Pro",
                Price = 1200,
                ImagePath = TestProductImageUrl,
                BrandId = 1,
                CategoryId = 1,
            };
            await this.productsRepository.AddAsync(secondProduct);
            await this.productsRepository.SaveChangesAsync();

            var result = this.productsService.GetAllProductsAsQueryeable<ProductDetailsViewModel>();

            var count = await result.CountAsync();
            var product = await result.FirstOrDefaultAsync();

            Assert.Equal(2, count);
            Assert.Equal(secondProduct.Name, product.Name);
        }

        [Fact]
        public async Task CheckIfGetProductViewModelByIdAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            var brand = await this.brandsRepository
                .All()
                .To<BrandDetailsViewModel>()
                .FirstOrDefaultAsync();

            var category = await this.categoriesRepository
                .All()
                .To<CategoryDetailsViewModel>()
                .FirstOrDefaultAsync();

            var expectedModel = new ProductDetailsViewModel
            {
                Id = this.firstProduct.Id,
                CreatedOn = this.firstProduct.CreatedOn,
                Name = "Canon eos 1100D",
                Code = 10600,
                Description = "Sample description for Canon eos 1100D",
                Price = 1500,
                ImagePath = TestProductImageUrl,
                Brand = brand,
                Category = category,
                Comments = new HashSet<PostProductCommentViewModel>(),
                Reviews = new HashSet<PostProductReviewViewModel>(),
            };

            expectedModel.CreatedOn = expectedModel.CreatedOn.ToLocalTime();

            var viewModel = await this.productsService.GetViewModelByIdAsync<ProductDetailsViewModel>(this.firstProduct.Id);
            viewModel.CreatedOn = viewModel.CreatedOn.ToLocalTime();

            var expectedObj = JsonConvert.SerializeObject(expectedModel);
            var actualResultObj = JsonConvert.SerializeObject(viewModel);

            Assert.Equal(expectedObj, actualResultObj);
        }

        [Fact]
        public async Task CheckIfGetViewModelByIdAsyncThrowsNullReferenceException()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () =>
                    await this.productsService.GetViewModelByIdAsync<ProductDetailsViewModel>(3));
            Assert.Equal(string.Format(ExceptionMessages.ProductNotFound, 3), exception.Message);
        }

        [Fact]
        public async Task CheckIfGetTopRatingProductsAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            var result = await this.productsService.GetTopRatingProductsAsync<ProductDetailsViewModel>();

            var count = result.Count();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task CheckIfGetByCategoryNameAsQueryableWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            var result = this.productsService.GetByCategoryNameAsQueryable("Cameras");

            var count = result.Count();
            var product = await result.FirstOrDefaultAsync();

            Assert.Equal(1, count);
            Assert.Equal("Cameras", product.Category.Name);
        }

        [Fact]
        public async Task CheckIfGetLastlyAddedProductAsyncWorksCorrectly()
        {
            this.SeedDatabase();
            await this.SeedProducts();

            var product = await this.productsService.GetLastlyAddedProductAsync<ProductDetailsViewModel>();
            var productBrand = await this.brandsRepository
                .All()
                .To<BrandDetailsViewModel>()
                .FirstOrDefaultAsync();
            var productCategory = await this.categoriesRepository
                .All()
                .To<CategoryDetailsViewModel>()
                .FirstOrDefaultAsync();

            Assert.Equal("Canon eos 1100D", product.Name);
            Assert.Equal(10600, product.Code);
            Assert.Equal("Sample description for Canon eos 1100D", product.Description);
            Assert.Equal(1500, product.Price);
            Assert.Equal(1, productBrand.Id);
            Assert.Equal(1, productCategory.Id);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.connection.Close();
                this.connection.Dispose();
                this.productsRepository.Dispose();
                this.brandsRepository.Dispose();
                this.categoriesRepository.Dispose();
                this.shoppingCartsRepository.Dispose();
                this.usersRepository.Dispose();
            }
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
