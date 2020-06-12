namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Categories;
    using PhotoPavilion.Models.ViewModels.Categories;

    public interface ICategoriesService : IBaseDataService
    {
        Task<CategoryDetailsViewModel> CreateAsync(CategoryCreateInputModel categoryCreateInputModel);

        Task EditAsync(CategoryEditViewModel categoryEditViewModel);

        Task<IEnumerable<TViewModel>> GetAllCategoriesAsync<TViewModel>();

        Task<TViewModel> GetCategoryAsync<TViewModel>(string name);
    }
}
