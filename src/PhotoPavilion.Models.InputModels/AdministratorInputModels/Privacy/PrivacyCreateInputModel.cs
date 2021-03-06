﻿namespace PhotoPavilion.Models.InputModels.AdministratorInputModels.Privacy
{
    using System.ComponentModel.DataAnnotations;

    using static PhotoPavilion.Models.Common.ModelValidation;
    using static PhotoPavilion.Models.Common.ModelValidation.Privacy;

    public class PrivacyCreateInputModel
    {
        [Display(Name = PageContentDisplayName)]
        [Required(ErrorMessage = EmptyFieldLengthError)]
        [StringLength(PageContentMaxLength, MinimumLength = PageContentMinLength, ErrorMessage = PageContentLengthError)]
        public string PageContent { get; set; }
    }
}
