namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Privacy;
    using PhotoPavilion.Models.ViewModels.Privacy;

    public interface IPrivacyService : IBaseDataService
    {
        Task<PrivacyDetailsViewModel> CreateAsync(PrivacyCreateInputModel privacyCreateInputModel);

        Task EditAsync(PrivacyEditViewModel privacyEditViewModel);

        Task<TViewModel> GetViewModelAsync<TViewModel>();
    }
}
