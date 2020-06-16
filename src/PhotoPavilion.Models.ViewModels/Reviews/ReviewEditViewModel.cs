namespace PhotoPavilion.Models.ViewModels.Reviews
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using PhotoPavilion.Services.Mapping;

    using static PhotoPavilion.Models.Common.ModelValidation;
    using static PhotoPavilion.Models.Common.ModelValidation.Review;

    using Review = PhotoPavilion.Data.Models.Review;

    public class ReviewEditViewModel : IMapFrom<Review>
    {
        public int Id { get; set; }

        [Required(ErrorMessage = EmptyFieldLengthError)]
        [StringLength(TitleMaxLength, MinimumLength = TitleMinLength, ErrorMessage = TitleLengthError)]
        public string Title { get; set; }

        [Required(ErrorMessage = EmptyFieldLengthError)]
        [StringLength(DescriptionMaxLength, MinimumLength = DescriptionMinLength, ErrorMessage = DescriptionLengthError)]
        public string Description { get; set; }

        public DateTime Date { get; set; }
    }
}
