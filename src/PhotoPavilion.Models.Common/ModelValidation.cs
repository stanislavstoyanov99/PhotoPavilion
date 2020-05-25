namespace PhotoPavilion.Models.Common
{
    public static class ModelValidation
    {
        public const string NameLengthError = "Name must be between {2} and {1} symbols";
        public const string EmptyFieldLengthError = "Please enter the field.";
        public const string IdDisplayName = "No.";

        public static class Product
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 40;

            public const int CodeMaxLength = int.MaxValue;

            public const int DescriptionMinLength = 50;
            public const int DescriptionMaxLength = 2000;

            public const string DescriptionError = "Description must be between {2} and {1} symbols";

            public const int ImageMinLength = 10;
            public const int ImageMaxLength = 500;

            public const string ImagePathError = "Image path must be between {2} and {1} symbols";

            public const int ImageMaxSize = 10 * 1024 * 1024;
        }

        public static class Brand
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 30;
            public const string BrandIdError = "Please select brand name.";
        }

        public static class Category
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 30;
            public const string CategoryIdError = "Please select category name.";
        }
    }
}
