namespace PhotoPavilion.Web.Areas.Administration.Views.Products
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using PhotoPavilion.Web.Areas.Administration.Views.Shared;

    public class ProductNavPages : AdminNavPages
    {
        public static string CreateProduct => "CreateProduct";

        public static string GetAll => "GetAll";

        public static string CreateProductNavClass(ViewContext viewContext) => PageNavClass(viewContext, CreateProduct);

        public static string GetAllNavClass(ViewContext viewContext) => PageNavClass(viewContext, GetAll);
    }
}
