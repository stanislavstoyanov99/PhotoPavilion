namespace PhotoPavilion.Services.Data.Tests
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;
    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Data.Repositories;
    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Brands;
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
