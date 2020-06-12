﻿namespace PhotoPavilion.Models.Common
{
    public static class ModelValidation
    {
        public const string NameLengthError = "Name must be between {2} and {1} symbols";
        public const string EmptyFieldLengthError = "Please enter the field.";
        public const string IdDisplayName = "No.";

        public static class Product
        {
            public const int NameMinLength = 2;
            public const int NameMaxLength = 100;

            public const int CodeMaxLength = 20000;

            public const int DescriptionMinLength = 50;
            public const int DescriptionMaxLength = 20000;

            public const string DescriptionError = "Description must be between {2} and {1} symbols";

            public const int ImageMinLength = 10;
            public const int ImageMaxLength = 500;

            public const string ImagePathError = "Image path must be between {2} and {1} symbols";

            public const int ImageMaxSize = 10 * 1024 * 1024;

            public const string ImageDisplayName = "Image";
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
            public const int DescriptionMinLength = 50;
            public const int DescriptionMaxLength = 500;
            public const string DescriptionError = "Description must be between {2} and {1} symbols";
            public const string CategoryIdError = "Please select category name.";
        }

        public static class Privacy
        {
            public const int PageContentMinLength = 1000;
            public const int PageContentMaxLength = 15000;

            public const string PageContentLengthError = "Page content must be between {2} and {1} symbols";
            public const string PageContentDisplayName = "Page Content";
        }
    }
}
