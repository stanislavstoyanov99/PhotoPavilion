namespace PhotoPavilion.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.InputModels.ProductReviews;
    using PhotoPavilion.Services.Data.Contracts;

    public class ProductReviewsController : Controller
    {
        private const string ReviewsSection = "#nav-reviews";
        private readonly IProductReviewsService productReviewsService;
        private readonly UserManager<PhotoPavilionUser> userManager;

        public ProductReviewsController(
            IProductReviewsService productReviewsService,
            UserManager<PhotoPavilionUser> userManager)
        {
            this.productReviewsService = productReviewsService;
            this.userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateProductReveiwInputModel input)
        {
            var userId = this.userManager.GetUserId(this.User);

            try
            {
                await this.productReviewsService.CreateAsync(input.ProductId, userId, input.Title, input.Description);
            }
            catch (ArgumentException aex)
            {
                return this.BadRequest(aex.Message);
            }

            return this.Redirect(this.Url.Action("Details", "Products", new { id = input.ProductId }) + ReviewsSection);
        }
    }
}
