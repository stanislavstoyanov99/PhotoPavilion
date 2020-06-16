namespace PhotoPavilion.Models.InputModels.ProductReviews
{
    using System.ComponentModel.DataAnnotations;

    using static PhotoPavilion.Models.Common.ModelValidation;

    public class CreateProductReveiwInputModel
    {
        public int ProductId { get; set; }

        [Required(ErrorMessage = EmptyFieldLengthError)]
        public string Title { get; set; }

        [Required(ErrorMessage = EmptyFieldLengthError)]
        public string Description { get; set; }
    }
}
