namespace PhotoPavilion.Models.InputModels.ProductComments
{
    using System.ComponentModel.DataAnnotations;

    using static PhotoPavilion.Models.Common.ModelValidation;

    public class CreateProductCommentInputModel
    {
        public int ProductId { get; set; }

        public int ParentId { get; set; }

        [Required(ErrorMessage = EmptyFieldLengthError)]
        public string Content { get; set; }
    }
}
