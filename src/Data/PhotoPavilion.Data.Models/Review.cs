namespace PhotoPavilion.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Common.Models;

    using static PhotoPavilion.Data.Common.DataValidation.Review;

    public class Review : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(TitleMaxLength)]
        public string Title { get; set; }

        [Required]
        [MaxLength(DescriptionMaxLength)]
        public string Description { get; set; }

        public string UserId { get; set; }

        public virtual PhotoPavilionUser User { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }
    }
}
