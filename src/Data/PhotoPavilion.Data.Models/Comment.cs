namespace PhotoPavilion.Data.Models
{
    using PhotoPavilion.Data.Common.Models;

    public class Comment : BaseDeletableModel<int>
    {
        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int? ParentId { get; set; }

        public virtual Comment Parent { get; set; }

        public string Content { get; set; }

        public string UserId { get; set; }

        public virtual PhotoPavilionUser User { get; set; }
    }
}
