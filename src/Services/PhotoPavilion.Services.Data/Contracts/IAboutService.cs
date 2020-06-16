﻿namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using PhotoPavilion.Models.InputModels.AdministratorInputModels.About;
    using PhotoPavilion.Models.ViewModels.About;

    public interface IAboutService : IBaseDataService
    {
        Task<FaqDetailsViewModel> CreateAsync(FaqCreateInputModel faqCreateInputModel);

        Task EditAsync(FaqEditViewModel faqEditViewModel);

        Task<IEnumerable<TEntity>> GetAllFaqsAsync<TEntity>();
    }
}
