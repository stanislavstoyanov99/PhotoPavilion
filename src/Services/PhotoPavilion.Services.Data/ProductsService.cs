﻿namespace PhotoPavilion.Services.Data
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

            bool doesProductExist = await this.productsRepository
                .All()
                .AnyAsync(p => p.Name == product.Name);
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
            var product = await this.productsRepository
                .All()
                .FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.ProductNotFound, id));
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

        public IQueryable<TViewModel> GetAllProductsAsQueryeable<TViewModel>()
        {
            var products = this.productsRepository
                .All()
                .OrderBy(x => x.Name)
                .ThenByDescending(x => x.CreatedOn)
                .To<TViewModel>();

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

        public async Task<IEnumerable<TViewModel>> GetTopRatingProductsAsync<TViewModel>(decimal rating = 0, int count = 0)
        {
            var topRatingProducts = await this.productsRepository
                .All()
                .Where(p => p.Ratings.Sum(x => x.Rate) > (double)rating)
                .OrderByDescending(p => p.Ratings.Sum(x => x.Rate))
                .ThenBy(p => (double)p.Price)
                .Take(count)
                .To<TViewModel>()
                .ToListAsync();

            return topRatingProducts;
        }

        public IQueryable<ProductDetailsViewModel> GetByCategoryNameAsQueryable(string name)
        {
            var productsByCategoryName = this.productsRepository
                .All()
                .Where(p => p.Category.Name == name)
                .To<ProductDetailsViewModel>();

            return productsByCategoryName;
        }

        public async Task<TViewModel> GetLastlyAddedProductAsync<TViewModel>()
        {
            var lastlyAddedProduct = await this.productsRepository
                .All()
                .OrderByDescending(x => x.CreatedOn)
                .To<TViewModel>()
                .FirstAsync();

            return lastlyAddedProduct;
        }
    }
}
