namespace PhotoPavilion.Web.Areas.Administration.Views.Brands
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    using PhotoPavilion.Web.Areas.Administration.Views.Shared;

    public class BrandNavPages : AdminNavPages
    {
        public static string CreateBrand => "CreateBrand";

        public static string GetAll => "GetAll";

        public static string CreateBrandNavClass(ViewContext viewContext) => PageNavClass(viewContext, CreateBrand);

        public static string GetAllNavClass(ViewContext viewContext) => PageNavClass(viewContext, GetAll);
    }
}
