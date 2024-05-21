namespace PhotoPavilion.Models.ViewModels.About
{
    using Ganss.Xss;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Services.Mapping;

    public class FaqDetailsViewModel : IMapFrom<FaqEntry>
    {
        public int Id { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public string SanitizedAnswer => new HtmlSanitizer().Sanitize(this.Answer);
    }
}
