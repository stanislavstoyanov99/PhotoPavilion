namespace PhotoPavilion.Web.Areas.Administration.Views.Privacy
{
    using Microsoft.AspNetCore.Mvc.Rendering;

    using PhotoPavilion.Web.Areas.Administration.Views.Shared;

    public class PrivacyNavPages : AdminNavPages
    {
        public static string CreatePrivacy => "CreatePrivacy";

        public static string EditPrivacy => "EditPrivacy";

        public static string DeletePrivacy => "DeletePrivacy";

        public static string CreatePrivacyNavClass(ViewContext viewContext) => PageNavClass(viewContext, CreatePrivacy);

        public static string EditPrivacyNavClass(ViewContext viewContext) => PageNavClass(viewContext, EditPrivacy);

        public static string DeletePrivacyNavClass(ViewContext viewContext) => PageNavClass(viewContext, DeletePrivacy);
    }
}
