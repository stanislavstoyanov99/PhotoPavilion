namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Brands;
    using PhotoPavilion.Models.ViewModels.Brands;

    public interface IBrandsService : IBaseDataService
    {
        Task<BrandDetailsViewModel> CreateAsync(BrandCreateInputModel brandCreateInputModel);

        Task EditAsync(BrandEditViewModel brandEditViewModel);

        Task<IEnumerable<TViewModel>> GetAllBrandsAsync<TViewModel>();
    }
}
