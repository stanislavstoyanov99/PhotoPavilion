namespace PhotoPavilion.Models.ViewModels.Privacy
{
    using System.ComponentModel.DataAnnotations;
    using Ganss.Xss;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Services.Mapping;

    using static PhotoPavilion.Models.Common.ModelValidation.Privacy;

    public class PrivacyDetailsViewModel : IMapFrom<Privacy>
    {
        public int Id { get; set; }

        [Display(Name = PageContentDisplayName)]
        public string PageContent { get; set; }

        public string SanitizedPageContent => new HtmlSanitizer().Sanitize(this.PageContent);
    }
}
