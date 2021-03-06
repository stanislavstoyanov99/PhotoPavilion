﻿namespace PhotoPavilion.Data.Common
{
    public static class DataValidation
    {
        public const int NameMaxLength = 30;
        public const int FullNameMaxLength = 60;

        public static class Product
        {
            public const int NameMaxLength = 100;
            public const int DescriptionMaxLength = 20000;
            public const int ImagePathMaxLength = 500;
        }

        public static class Brand
        {
            public const int NameMaxLength = 30;
        }

        public static class Category
        {
            public const int DescriptionMaxLength = 500;
            public const int NameMaxLength = 30;
        }

        public static class Review
        {
            public const int TitleMaxLength = 100;
            public const int DescriptionMaxLength = 1500;
        }

        public static class Privacy
        {
            public const int ContentPageMaxLength = 15000;
        }

        public static class FaqEntry
        {
            public const int QuestionMaxLength = 100;
            public const int AnswerMaxLength = 1000;
        }

        public static class ContactFormEntry
        {
            public const int SubjectMaxLength = 100;
            public const int ContentMaxLength = 10000;
        }
    }
}
