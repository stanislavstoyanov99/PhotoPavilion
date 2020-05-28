namespace PhotoPavilion.Web.Areas.Administration.Views.Shared
{
    using System;

    using Microsoft.AspNetCore.Mvc.Rendering;

    // TODO - add all admin pages
    public class AdminNavPages
    {
        public static string Products => "Movies";

        public static string Categories => "Cinemas";

        public static string Brands => "Genres";

        public static string Privacy => "Privacy";

        public static string Contacts => "Contacts";

        public static string ProductsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Products);

        public static string CategoriesNavClass(ViewContext viewContext) => PageNavClass(viewContext, Categories);

        public static string BrandsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Brands);

        public static string PrivacyNavClass(ViewContext viewContext) => PageNavClass(viewContext, Privacy);

        public static string ContactsNavClass(ViewContext viewContext) => PageNavClass(viewContext, Contacts);

        protected static string PageNavClass(ViewContext viewContext, string page)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string
                ?? System.IO.Path.GetFileNameWithoutExtension(viewContext.ActionDescriptor.DisplayName);

            return string.Equals(activePage, page, StringComparison.OrdinalIgnoreCase) ? "active" : null;
        }
    }
}
