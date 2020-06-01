namespace PhotoPavilion.Web.Areas.Administration.Views.Categories
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using PhotoPavilion.Web.Areas.Administration.Views.Shared;

    public class CategoryNavPages : AdminNavPages
    {
        public static string CreateCategory => "CreateCategory";

        public static string GetAll => "GetAll";

        public static string CreateCategoryNavClass(ViewContext viewContext) => PageNavClass(viewContext, CreateCategory);

        public static string GetAllNavClass(ViewContext viewContext) => PageNavClass(viewContext, GetAll);
    }
}
