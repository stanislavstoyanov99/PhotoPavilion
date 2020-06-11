namespace PhotoPavilion.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Common.Models;

    using static PhotoPavilion.Data.Common.DataValidation.Privacy;

    public class Privacy : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(ContentPageMaxLength)]
        public string PageContent { get; set; }
    }
}
