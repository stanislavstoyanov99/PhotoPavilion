namespace PhotoPavilion.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Data.Common.Models;

    using static PhotoPavilion.Data.Common.DataValidation.FaqEntry;

    public class FaqEntry : BaseDeletableModel<int>
    {
        [Required]
        [MaxLength(QuestionMaxLength)]
        public string Question { get; set; }

        [Required]
        [MaxLength(AnswerMaxLength)]
        public string Answer { get; set; }
    }
}
