namespace PhotoPavilion.Models.ViewModels.Categories
{
    using System.ComponentModel.DataAnnotations;
    using Ganss.Xss;
    using PhotoPavilion.Services.Mapping;

    using static PhotoPavilion.Models.Common.ModelValidation;
    using Category = Data.Models.Category;

    public class CategoryDetailsViewModel : IMapFrom<Category>
    {
        [Display(Name = IdDisplayName)]
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ShortDescription
        {
            get
            {
                var shortDescription = this.Description;
                return shortDescription.Length > 200
                        ? shortDescription[..200] + " ..."
                        : shortDescription;
            }
        }

        public string SanitizedDescription => new HtmlSanitizer().Sanitize(this.Description);

        public string SanitizedShortDescription => new HtmlSanitizer().Sanitize(this.ShortDescription);
    }
}
