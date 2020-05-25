namespace PhotoPavilion.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Categories;
    using PhotoPavilion.Models.ViewModels.Categories;
    using PhotoPavilion.Services.Data.Contracts;

    // TODO - finish this service
    public class CategoriesService : ICategoriesService
    {
        private readonly IDeletableEntityRepository<Category> categoriesRepository;

        public CategoriesService(IDeletableEntityRepository<Category> categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public Task<CategoryDetailsViewModel> CreateAsync(CategoryCreateInputModel categoryCreateInputModel)
        {
            throw new NotImplementedException();
        }

        public Task DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task EditAsync(CategoryEditViewModel categoryEditViewModel)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TViewModel>> GetAllCategoriesAsync<TViewModel>()
        {
            throw new NotImplementedException();
        }

        public Task<TViewModel> GetViewModelByIdAsync<TViewModel>(int id)
        {
            throw new NotImplementedException();
        }
    }
}
