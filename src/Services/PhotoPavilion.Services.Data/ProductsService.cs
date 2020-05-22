namespace PhotoPavilion.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Products;
    using PhotoPavilion.Models.ViewModels.Products;
    using PhotoPavilion.Services.Data.Contracts;

    public class ProductsService : IProductsService
    {
        private readonly IDeletableEntityRepository<Product> productsRepository;

        public ProductsService(IDeletableEntityRepository<Product> productsRepository)
        {
            this.productsRepository = productsRepository;
        }

        public Task<ProductDetailsViewModel> CreateAsync(ProductCreateInputModel productCreateInputModel)
        {
            throw new NotImplementedException();
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
