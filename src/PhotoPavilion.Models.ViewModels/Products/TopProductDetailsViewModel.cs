namespace PhotoPavilion.Models.ViewModels.Products
{
    using Ganss.Xss;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Services.Mapping;

    public class TopProductDetailsViewModel : IMapFrom<Product>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string ImagePath { get; set; }

        public string Description { get; set; }

        public string ShortDescription
        {
            get
            {
                var shortDescription = this.Description;
                return shortDescription.Length > 150
                        ? shortDescription[..150] + " ..."
                        : shortDescription;
            }
        }

        public string SanitizedShortDescription => new HtmlSanitizer().Sanitize(this.ShortDescription);
    }
}
