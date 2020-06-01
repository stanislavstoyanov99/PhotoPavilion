namespace PhotoPavilion.Data.Common
{
    public static class DataValidation
    {
        public const int NameMaxLength = 30;
        public const int FullNameMaxLength = 60;

        public static class Product
        {
            public const int NameMaxLength = 40;
            public const int DescriptionMaxLength = 20000;
            public const int ImagePathMaxLength = 500;
        }

        public static class Brand
        {
            public const int NameMaxLength = 30;
        }

        public static class Category
        {
            public const int NameMaxLength = 30;
        }

        public static class Review
        {
            public const int TitleMaxLength = 100;
            public const int DescriptionMaxLength = 1500;
        }
    }
}
