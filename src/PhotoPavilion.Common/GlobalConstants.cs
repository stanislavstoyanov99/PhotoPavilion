﻿namespace PhotoPavilion.Common
{
    public static class GlobalConstants
    {
        public const string SystemName = "Photo Pavilion";

        public const string SystemEmail = "slavkata_99@abv.bg";

        public const string AdministratorRoleName = "Administrator";

        public const string AdministratorUsername = "Admin";

        public const string AdministratorEmail = "slavkata_99@abv.bg";

        public const string AdministratorPassword = "123456";

        public const string AdministratorFullName = "Stanislav";

        public const string UserRoleName = "User";

        public const string AllowedExtensionsErrorMessage = "This photo extension is not allowed.";

        public const string MaxFileSizeErrorMessage = "Maximum allowed file size is {0} megabytes.";

        public const string OnlinePaymentMethod = "online";

        public const string CashPaymentMethod = "cash";

        public static readonly string[] AllowedImageExtensions = { ".jpg", ".png", ".jpeg", ".gif" };
    }
}
