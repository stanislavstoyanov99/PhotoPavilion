namespace PhotoPavilion.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.InputModels.AdministratorInputModels.Brands;
    using PhotoPavilion.Models.ViewModels.Brands;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;
    using PhotoPavilion.Services.Mapping;

    public class BrandsService : IBrandsService
    {
        private readonly IDeletableEntityRepository<Brand> brandsRepository;

        public BrandsService(IDeletableEntityRepository<Brand> brandsRepository)
        {
            this.brandsRepository = brandsRepository;
        }

        public async Task<BrandDetailsViewModel> CreateAsync(BrandCreateInputModel brandCreateInputModel)
        {
            var brand = new Brand
            {
                Name = brandCreateInputModel.Name,
            };

            bool doesBrandExist = await this.brandsRepository
                .All()
                .AnyAsync(b => b.Name == brand.Name);
            if (doesBrandExist)
            {
                throw new ArgumentException(
                    string.Format(ExceptionMessages.BrandAlreadyExists, brand.Name));
            }

            await this.brandsRepository.AddAsync(brand);
            await this.brandsRepository.SaveChangesAsync();

            var viewModel = await this.GetViewModelByIdAsync<BrandDetailsViewModel>(brand.Id);

            return viewModel;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var brand = await this.brandsRepository
                .All()
                .FirstOrDefaultAsync(b => b.Id == id);
            if (brand == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.BrandNotFound, id));
            }

            brand.IsDeleted = true;
            brand.DeletedOn = DateTime.UtcNow;

            this.brandsRepository.Update(brand);
            await this.brandsRepository.SaveChangesAsync();
        }

        public async Task EditAsync(BrandEditViewModel brandEditViewModel)
        {
            var brand = await this.brandsRepository
                .All()
                .FirstOrDefaultAsync(b => b.Id == brandEditViewModel.Id);

            if (brand == null)
            {
                throw new NullReferenceException(
                    string.Format(ExceptionMessages.BrandNotFound, brandEditViewModel.Id));
            }

            brand.Name = brandEditViewModel.Name;
            brand.ModifiedOn = DateTime.UtcNow;

            this.brandsRepository.Update(brand);
            await this.brandsRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<TViewModel>> GetAllBrandsAsync<TViewModel>()
        {
            var brands = await this.brandsRepository
                .All()
                .OrderBy(b => b.Name)
                .To<TViewModel>()
                .ToListAsync();

            return brands;
        }

        public async Task<TViewModel> GetViewModelByIdAsync<TViewModel>(int id)
        {
            var brandViewModel = await this.brandsRepository
                .All()
                .Where(b => b.Id == id)
                .To<TViewModel>()
                .FirstOrDefaultAsync();

            if (brandViewModel == null)
            {
                throw new NullReferenceException(string.Format(ExceptionMessages.BrandNotFound, id));
            }

            return brandViewModel;
        }
    }
}
