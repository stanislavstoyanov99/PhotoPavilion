namespace PhotoPavilion.Web.Areas.Administration.Controllers
{
    using PhotoPavilion.Common;
    using PhotoPavilion.Web.Controllers;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
    }
}
