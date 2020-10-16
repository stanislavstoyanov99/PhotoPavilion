namespace PhotoPavilion.Services.Data.Tests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using PhotoPavilion.Data;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Repositories;
    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Brands;
    using PhotoPavilion.Models.ViewModels.Brands;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;

    using Xunit;

    public class BrandsServiceTests : IDisposable
    {
        private readonly IBrandsService brandsService;
        private EfDeletableEntityRepository<Brand> brandsRepository;
        private SqliteConnection connection;

        private Brand firstBrand;

        public BrandsServiceTests()
        {
            this.InitializeMapper();
            this.InitializeDatabaseAndRepositories();
            this.InitializeFields();

            this.brandsService = new BrandsService(this.brandsRepository);
        }

        [Fact]
        public async Task TestAddingBrand()
        {
            var model = new BrandCreateInputModel
            {
                Name = "Nikon",
            };

            await this.brandsService.CreateAsync(model);
            var count = await this.brandsRepository.All().CountAsync();

            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckSettingOfBrandProperties()
        {
            var model = new BrandCreateInputModel
            {
                Name = "Sony",
            };

            await this.brandsService.CreateAsync(model);

            var brand = await this.brandsRepository.All().FirstOrDefaultAsync();

            Assert.Equal("Sony", brand.Name);
        }

        [Fact]
        public async Task CheckIfAddingBrandThrowsArgumentException()
        {
            this.SeedDatabase();

            var brand = new BrandCreateInputModel
            {
                Name = this.firstBrand.Name,
            };

            var exception = await Assert
                .ThrowsAsync<ArgumentException>(async () => await this.brandsService.CreateAsync(brand));
            Assert.Equal(string.Format(ExceptionMessages.BrandAlreadyExists, brand.Name), exception.Message);
        }

        [Fact]
        public async Task CheckIfAddingBrandReturnsViewModel()
        {
            var brand = new BrandCreateInputModel
            {
                Name = "Pentax",
            };

            var viewModel = await this.brandsService.CreateAsync(brand);
            var dbEntry = await this.brandsRepository.All().FirstOrDefaultAsync();

            Assert.Equal(dbEntry.Id, viewModel.Id);
            Assert.Equal(dbEntry.Name, viewModel.Name);
        }

        [Fact]
        public async Task CheckIfDeletingBrandWorksCorrectly()
        {
            this.SeedDatabase();

            await this.brandsService.DeleteByIdAsync(this.firstBrand.Id);

            var count = await this.brandsRepository.All().CountAsync();

            Assert.Equal(0, count);
        }

        [Fact]
        public async Task CheckIfDeletingBrandReturnsNullReferenceException()
        {
            this.SeedDatabase();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () => await this.brandsService.DeleteByIdAsync(3));
            Assert.Equal(string.Format(ExceptionMessages.BrandNotFound, 3), exception.Message);
        }

        [Fact]
        public async Task CheckIfEditingBrandWorksCorrectly()
        {
            this.SeedDatabase();

            var brandEditViewModel = new BrandEditViewModel
            {
                Id = this.firstBrand.Id,
                Name = "Sony",
            };

            await this.brandsService.EditAsync(brandEditViewModel);

            Assert.Equal(brandEditViewModel.Id, this.firstBrand.Id);
            Assert.Equal(brandEditViewModel.Name, this.firstBrand.Name);
        }

        [Fact]
        public async Task CheckIfEditingBrandReturnsNullReferenceException()
        {
            this.SeedDatabase();

            var brandEditViewModel = new BrandEditViewModel
            {
                Id = 3,
                Name = "Sony",
            };

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () => await this.brandsService.EditAsync(brandEditViewModel));
            Assert.Equal(string.Format(ExceptionMessages.BrandNotFound, brandEditViewModel.Id), exception.Message);
        }

        [Fact]
        public async Task CheckIfGetAllBrandsAsyncWorksCorrectly()
        {
            this.SeedDatabase();

            var result = await this.brandsService.GetAllBrandsAsync<BrandDetailsViewModel>();

            var count = result.Count();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckIfGetAllBrandsAsQueryeableWorksCorrectly()
        {
            this.SeedDatabase();

            var result = this.brandsService.GetAllBrandsAsQueryeable<BrandDetailsViewModel>();

            var count = await result.CountAsync();
            Assert.Equal(1, count);
        }

        [Fact]
        public async Task CheckIfGetBrandViewModelByIdAsyncWorksCorrectly()
        {
            this.SeedDatabase();

            var expectedModel = new BrandDetailsViewModel
            {
                Id = this.firstBrand.Id,
                Name = this.firstBrand.Name,
            };

            var viewModel = await this.brandsService.GetViewModelByIdAsync<BrandDetailsViewModel>(this.firstBrand.Id);

            var expectedObj = JsonConvert.SerializeObject(expectedModel);
            var actualResultObj = JsonConvert.SerializeObject(viewModel);

            Assert.Equal(expectedObj, actualResultObj);
        }

        [Fact]
        public async Task CheckIfGetViewModelByIdAsyncThrowsNullReferenceException()
        {
            this.SeedDatabase();

            var exception = await Assert
                .ThrowsAsync<NullReferenceException>(async () => await this.brandsService.GetViewModelByIdAsync<BrandDetailsViewModel>(3));
            Assert.Equal(string.Format(ExceptionMessages.BrandNotFound, 3), exception.Message);
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

            this.brandsRepository = new EfDeletableEntityRepository<Brand>(dbContext);
        }

        private void InitializeFields()
        {
            this.firstBrand = new Brand
            {
                Name = "Canon",
            };
        }

        private async void SeedDatabase()
        {
            await this.SeedBrands();
        }

        private async Task SeedBrands()
        {
            await this.brandsRepository.AddAsync(this.firstBrand);

            await this.brandsRepository.SaveChangesAsync();
        }

        private void InitializeMapper() => AutoMapperConfig.
            RegisterMappings(Assembly.Load("PhotoPavilion.Models.ViewModels"));
    }
}
