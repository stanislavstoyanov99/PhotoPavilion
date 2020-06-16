namespace PhotoPavilion.Web.Areas.Administration.Views.About
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using PhotoPavilion.Web.Areas.Administration.Views.Shared;

    public class FaqNavPages : AdminNavPages
    {
        public static string CreateFaq => "CreateFaq";

        public static string GetAll => "GetAll";

        public static string CreateFaqNavClass(ViewContext viewContext) => PageNavClass(viewContext, CreateFaq);

        public static string GetAllNavClass(ViewContext viewContext) => PageNavClass(viewContext, GetAll);
    }
}
