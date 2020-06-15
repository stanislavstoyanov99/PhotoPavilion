namespace PhotoPavilion.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.InputModels.ProductComments;
    using PhotoPavilion.Services.Data.Contracts;

    public class ProductCommentsController : Controller
    {
        private readonly IProductCommentsService productCommentsService;
        private readonly UserManager<PhotoPavilionUser> userManager;

        public ProductCommentsController(
            IProductCommentsService commentsService,
            UserManager<PhotoPavilionUser> userManager)
        {
            this.productCommentsService = commentsService;
            this.userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateProductCommentInputModel input)
        {
            var parentId = input.ParentId == 0 ? (int?)null : input.ParentId;

            if (parentId.HasValue)
            {
                if (!await this.productCommentsService.IsInProductId(parentId.Value, input.ProductId))
                {
                    return this.BadRequest();
                }
            }

            var userId = this.userManager.GetUserId(this.User);

            try
            {
                await this.productCommentsService.CreateAsync(input.ProductId, userId, input.Content, parentId);
            }
            catch (ArgumentException aex)
            {
                return this.BadRequest(aex.Message);
            }

            return this.Redirect(this.Url.Action("Details", "Products", new { id = input.ProductId }) + "#nav-comments");
        }
    }
}
