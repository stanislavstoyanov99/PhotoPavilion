﻿namespace PhotoPavilion.Models.ViewModels.Brands
{
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Services.Mapping;

    using static PhotoPavilion.Models.Common.ModelValidation;
    using Brand = Data.Models.Brand;

    public class BrandDetailsViewModel : IMapFrom<Brand>
    {
        [Display(Name = IdDisplayName)]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
