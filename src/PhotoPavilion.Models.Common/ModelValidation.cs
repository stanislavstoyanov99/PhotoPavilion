namespace PhotoPavilion.Models.Common
{
    public static class ModelValidation
    {
        public const string NameLengthError = "Name must be between {2} and {1} symbols";
        public const string EmptyFieldLengthError = "Please enter the field.";
        public const string IdDisplayName = "No.";

        public static class Product
        {
            public const string NameDisplay = "Product name";
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

        public static class ShoppingCartProduct
        {
            public const string ProductNameDisplayName = "Product name";
            public const string ProductPriceDisplayName = "Product price";
            public const string ProductCreatedOnDisplayName = "Product publish date";
        }

        public static class OrderProduct
        {
            public const string OrderIdDisplay = "#";
            public const string UserFullNameDisplay = "User full name";
            public const string CreatedOnNameDisplay = "Date of purchase";
            public const string OrderStatusNameDisplay = "Order status";
        }

        public static class FaqEntry
        {
            public const int QuestionMinLength = 10;
            public const int QuestionMaxLength = 100;

            public const int AnswerMinLength = 10;
            public const int AnswerMaxLength = 1000;

            public const string QuestionLengthError = "Question must be between {2} and {1} symbols";
            public const string AnswerLengthError = "Answer must be between {2} and {1} symbols";
        }

        public static class Review
        {
            public const int TitleMinLength = 10;
            public const int TitleMaxLength = 100;
            public const int DescriptionMinLength = 20;
            public const int DescriptionMaxLength = 1500;
            public const string ReviewsDisplayName = "Reviews";

            public const string DescriptionLengthError = "Description must be between {2} and {1} symbols";
            public const string TitleLengthError = "Title must be between {2} and {1} symbols";
        }

        public static class ContactFormEntry
        {
            public const int FirstNameMinLength = 3;
            public const int FirstNameMaxLength = 30;
            public const int LastNameMinLength = 3;
            public const int LastNameMaxLength = 30;

            public const int SubjectMaxLength = 100;
            public const int SubjectMinLegth = 5;

            public const int ContentMaxLength = 10000;
            public const int ContentMinLegth = 20;

            public const string FirstNameLengthError = "First name must be between {2} and {1} symbols";
            public const string LastNameLengthError = "Last name must be between {2} and {1} symbols";
            public const string SubjectLengthError = "Subject must be between {2} and {1} symbols";
            public const string ContentLengthError = "Content must be between {2} and {1} symbols";

            public const string FirstNameDisplayName = "First Name";
            public const string LastNameDispalyName = "Last Name";
        }
    }
}
