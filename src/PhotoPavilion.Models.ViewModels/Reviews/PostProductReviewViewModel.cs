namespace PhotoPavilion.Models.ViewModels.Reviews
{
    using Ganss.Xss;

    using System;

    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Services.Mapping;

    public class PostProductReviewViewModel : IMapFrom<Review>
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string SanitizedDescription => new HtmlSanitizer().Sanitize(this.Description);

        public string UserUserName { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
