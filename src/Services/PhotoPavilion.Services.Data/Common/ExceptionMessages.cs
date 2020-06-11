namespace PhotoPavilion.Services.Data.Common
{
    public static class ExceptionMessages
    {
        public const string ProductAlreadyExists = "Product with name {0} already exists";

        public const string ProductNotFound = "Product with id {0} is not found.";

        public const string BrandAlreadyExists = "Brand with name {0} already exists";

        public const string BrandNotFound = "Brand with id {0} is not found.";

        public const string CategoryAlreadyExists = "Category with name {0} already exists";

        public const string CategoryNotFound = "Category with id {0} is not found.";

        public const string PrivacyAlreadyExists = "Privacy with page content {0} already exists";

        public const string PrivacyNotFound = "Privacy with id {0} is not found.";

        public const string PrivacyViewModelNotFound = "Privacy view model is not found.";
    }
}
