﻿namespace PhotoPavilion.Web.Areas.Identity.Pages.Account.InputModels
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Common;
    using PhotoPavilion.Data.Models.Enumerations;

    public class ExternalLoginInputModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = nameof(Username))]
        public string Username { get; set; }

        [Required]
        [MaxLength(DataValidation.FullNameMaxLength, ErrorMessage = "The {0} must be max {1} characters long.")]
        [Display(Name = "Full name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public string SelectedGender { get; set; }

        public IEnumerable<string> Genders { get; set; } = new[]
        {
            nameof(Gender.Male),
            nameof(Gender.Female),
        };
    }
}
