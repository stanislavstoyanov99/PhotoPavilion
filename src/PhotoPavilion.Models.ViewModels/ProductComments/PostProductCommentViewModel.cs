namespace PhotoPavilion.Models.ViewModels.ProductComments
{
    using System;
    using Ganss.Xss;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Services.Mapping;

    public class PostProductCommentViewModel : IMapFrom<Comment>
    {
        public int Id { get; set; }

        public int? ParentId { get; set; }

        public string Content { get; set; }

        public string SanitizedContent => new HtmlSanitizer().Sanitize(this.Content);

        public DateTime CreatedOn { get; set; }

        public string UserUserName { get; set; }

        public string UserFullName { get; set; }
    }
}
