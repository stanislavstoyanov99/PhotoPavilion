namespace PhotoPavilion.Models.ViewModels.Privacy
{
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Services.Mapping;

    using static PhotoPavilion.Models.Common.ModelValidation;
    using static PhotoPavilion.Models.Common.ModelValidation.Privacy;

    using Privacy = PhotoPavilion.Data.Models.Privacy;

    public class PrivacyEditViewModel : IMapFrom<Privacy>
    {
        public int Id { get; set; }

        [Display(Name = PageContentDisplayName)]
        [Required(ErrorMessage = EmptyFieldLengthError)]
        [StringLength(PageContentMaxLength, MinimumLength = PageContentMinLength, ErrorMessage = PageContentLengthError)]
        public string PageContent { get; set; }
    }
}
