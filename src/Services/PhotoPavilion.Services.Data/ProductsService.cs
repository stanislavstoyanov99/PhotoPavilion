namespace PhotoPavilion.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Products;
    using PhotoPavilion.Models.ViewModels.Products;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;

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
                .FirstOrDefaultAsync(b => b.Id == productCreateInputModel.BrandId);

            if (brand == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.BrandNotFound, productCreateInputModel.BrandId));
            }

            var category = await this.categoriesRepository
                .All()
                .FirstOrDefaultAsync(c => c.Id == productCreateInputModel.CategoryId);

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

        public async Task DeleteByIdAsync(int id)
        {
            var product = await this.productsRepository.All().FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.CategoryNotFound, id));
            }

            product.IsDeleted = true;
            product.DeletedOn = DateTime.UtcNow;
            this.productsRepository.Update(product);
            await this.productsRepository.SaveChangesAsync();
        }

        public async Task EditAsync(ProductEditViewModel productEditViewModel)
        {
            var product = await this.productsRepository
                .All()
                .FirstOrDefaultAsync(p => p.Id == productEditViewModel.Id);

            if (product == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.ProductNotFound, productEditViewModel.Id));
            }

            if (productEditViewModel.Image != null)
            {
                var newImageUrl = await this.cloudinaryService
                    .UploadAsync(productEditViewModel.Image, productEditViewModel.Name);
                product.ImagePath = newImageUrl;
            }

            product.Name = productEditViewModel.Name;
            product.Code = productEditViewModel.Code;
            product.Description = productEditViewModel.Description;
            product.Price = productEditViewModel.Price;
            product.BrandId = productEditViewModel.BrandId;
            product.CategoryId = productEditViewModel.CategoryId;
            product.ModifiedOn = DateTime.UtcNow;

            await this.productsRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TViewModel>> GetAllProductsAsync<TViewModel>()
        {
            var products = await this.productsRepository
                .All()
                .To<TViewModel>()
                .ToListAsync();

            return products;
        }

        public async Task<TViewModel> GetViewModelByIdAsync<TViewModel>(int id)
        {
            var product = await this.productsRepository
                .All()
                .Where(p => p.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            if (product == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.ProductNotFound, id));
            }

            return product;
        }
    }
}
