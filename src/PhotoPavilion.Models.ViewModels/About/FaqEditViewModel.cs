namespace PhotoPavilion.Models.ViewModels.About
{
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Services.Mapping;

    using static PhotoPavilion.Models.Common.ModelValidation;
    using static PhotoPavilion.Models.Common.ModelValidation.FaqEntry;

    using FaqEntry = PhotoPavilion.Data.Models.FaqEntry;

    public class FaqEditViewModel : IMapFrom<FaqEntry>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = EmptyFieldLengthError)]
        [StringLength(QuestionMaxLength, MinimumLength = QuestionMinLength, ErrorMessage = QuestionLengthError)]
        public string Question { get; set; }

        [Required(ErrorMessage = EmptyFieldLengthError)]
        [StringLength(AnswerMaxLength, MinimumLength = AnswerMinLength, ErrorMessage = AnswerLengthError)]
        public string Answer { get; set; }
    }
}
