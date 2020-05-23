namespace PhotoPavilion.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Products;
    using PhotoPavilion.Models.ViewModels.Products;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;

    public class ProductsService : IProductsService
    {
        private readonly IDeletableEntityRepository<Product> productsRepository;
        private readonly IDeletableEntityRepository<Brand> brandsRepository;
        private readonly IDeletableEntityRepository<Category> categoriesRepository;
        private readonly ICloudinaryService cloudinaryService;

        public ProductsService(
            IDeletableEntityRepository<Product> productsRepository,
            IDeletableEntityRepository<Brand> brandsRepository,
            IDeletableEntityRepository<Category> categoriesRepository,
            ICloudinaryService cloudinaryService)
        {
            this.productsRepository = productsRepository;
            this.brandsRepository = brandsRepository;
            this.categoriesRepository = categoriesRepository;
            this.cloudinaryService = cloudinaryService;
        }

        public async Task<ProductDetailsViewModel> CreateAsync(ProductCreateInputModel productCreateInputModel)
        {
            var brand = await this.brandsRepository
                .All()
                .FirstOrDefaultAsync(d => d.Id == productCreateInputModel.BrandId);

            if (brand == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.BrandNotFound, productCreateInputModel.BrandId));
            }

            var category = await this.categoriesRepository
                .All()
                .FirstOrDefaultAsync(d => d.Id == productCreateInputModel.CategoryId);

            if (category == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.CategoryNotFound, productCreateInputModel.CategoryId));
            }

            var imageUrl = await this.cloudinaryService
                .UploadAsync(productCreateInputModel.Image, productCreateInputModel.Name + Suffixes.ProductSuffix);

            var product = new Product
            {
                Name = productCreateInputModel.Name,
                Code = productCreateInputModel.Code,
                Description = productCreateInputModel.Description,
                Price = productCreateInputModel.Price,
                ImagePath = imageUrl,
                Brand = brand,
                Category = category,
            };

            bool doesProductExist = await this.productsRepository.All().AnyAsync(x => x.Name == product.Name);
            if (doesProductExist)
            {
                throw new ArgumentException(
                    string.Format(ExceptionMessages.ProductAlreadyExists, product.Name));
            }

            await this.productsRepository.AddAsync(product);
            await this.productsRepository.SaveChangesAsync();

            var viewModel = await this.GetViewModelByIdAsync<ProductDetailsViewModel>(product.Id);

            return viewModel;
        }

        public Task DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task EditAsync(ProductEditViewModel productEditViewModel)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TViewModel>> GetAllProductsAsync<TViewModel>()
        {
            throw new NotImplementedException();
        }

        public Task<TViewModel> GetViewModelByIdAsync<TViewModel>(int id)
        {
            throw new NotImplementedException();
        }
    }
}
