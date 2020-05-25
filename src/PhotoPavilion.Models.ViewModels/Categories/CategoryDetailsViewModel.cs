﻿namespace PhotoPavilion.Models.ViewModels.Categories
{
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Services.Mapping;

    using static PhotoPavilion.Models.Common.ModelValidation;
    using Category = Data.Models.Category;

    public class CategoryDetailsViewModel : IMapFrom<Category>
    {
        [Display(Name = IdDisplayName)]
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
