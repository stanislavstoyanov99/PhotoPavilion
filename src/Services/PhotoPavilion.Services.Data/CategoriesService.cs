namespace PhotoPavilion.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Categories;
    using PhotoPavilion.Models.ViewModels.Categories;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;

    public class CategoriesService : ICategoriesService
    {
        private readonly IDeletableEntityRepository<Category> categoriesRepository;

        public CategoriesService(IDeletableEntityRepository<Category> categoriesRepository)
        {
            this.categoriesRepository = categoriesRepository;
        }

        public async Task<CategoryDetailsViewModel> CreateAsync(CategoryCreateInputModel categoryCreateInputModel)
        {
            var category = new Category
            {
                Name = categoryCreateInputModel.Name,
                Description = categoryCreateInputModel.Description,
            };

            bool doesCategoryExist = await this.categoriesRepository
                .All()
                .AnyAsync(c => c.Name == category.Name);
            if (doesCategoryExist)
            {
                throw new ArgumentException(
                    string.Format(ExceptionMessages.CategoryAlreadyExists, category.Name));
            }

            await this.categoriesRepository.AddAsync(category);
            await this.categoriesRepository.SaveChangesAsync();

            var viewModel = await this.GetViewModelByIdAsync<CategoryDetailsViewModel>(category.Id);

            return viewModel;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var category = await this.categoriesRepository
                .All()
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.CategoryNotFound, id));
            }

            category.IsDeleted = true;
            category.DeletedOn = DateTime.UtcNow;

            this.categoriesRepository.Update(category);
            await this.categoriesRepository.SaveChangesAsync();
        }

        public async Task EditAsync(CategoryEditViewModel categoryEditViewModel)
        {
            var category = await this.categoriesRepository
                .All()
                .FirstOrDefaultAsync(c => c.Id == categoryEditViewModel.Id);

            if (category == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.CategoryNotFound, categoryEditViewModel.Id));
            }

            category.Name = categoryEditViewModel.Name;
            category.Description = categoryEditViewModel.Description;
            category.ModifiedOn = DateTime.UtcNow;

            this.categoriesRepository.Update(category);
            await this.categoriesRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TViewModel>> GetAllCategoriesAsync<TViewModel>()
        {
            var categories = await this.categoriesRepository
                .All()
                .OrderBy(c => c.Name)
                .To<TViewModel>()
                .ToListAsync();

            return categories;
        }

        public async Task<TViewModel> GetCategoryAsync<TViewModel>(string name)
        {
            var category = await this.categoriesRepository
                .All()
                .Where(c => c.Name == name)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            return category;
        }

        public async Task<TViewModel> GetViewModelByIdAsync<TViewModel>(int id)
        {
            var categoryViewModel = await this.categoriesRepository
                .All()
                .Where(c => c.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            if (categoryViewModel == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.CategoryNotFound, id));
            }

            return categoryViewModel;
        }
    }
}
