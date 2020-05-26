﻿namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Products;
    using PhotoPavilion.Models.ViewModels.Products;

    public interface IProductsService : IBaseDataService
    {
        Task<ProductDetailsViewModel> CreateAsync(ProductCreateInputModel productCreateInputModel);

        Task EditAsync(ProductEditViewModel productEditViewModel);

        Task<IEnumerable<TViewModel>> GetAllProductsAsync<TViewModel>();
    }
}