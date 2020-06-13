namespace PhotoPavilion.Web.Controllers
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Models.InputModels.Ratings;
    using PhotoPavilion.Models.ViewModels.Ratings;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;

    [ApiController]
    [Route("api/[controller]")]
    public class RatingsController : ControllerBase
    {
        private readonly IRatingsService ratingsService;
        private readonly UserManager<PhotoPavilionUser> userManager;

        public RatingsController(IRatingsService ratingsService, UserManager<PhotoPavilionUser> userManager)
        {
            this.ratingsService = ratingsService;
            this.userManager = userManager;
        }

        [HttpPost]
        public async Task<ActionResult<StarRatingResponseModel>> Post(RatingInputModel input)
        {
            var userId = this.userManager.GetUserId(this.User);
            var starRatingResponseModel = new StarRatingResponseModel();

            if (userId == null)
            {
                starRatingResponseModel.AuthenticateErrorMessage = ExceptionMessages.AuthenticatedErrorMessage;
                starRatingResponseModel.StarRatingsSum = await this.ratingsService.GetStarRatingsAsync(input.ProductId);

                return starRatingResponseModel;
            }

            try
            {
                await this.ratingsService.VoteAsync(input.ProductId, userId, input.Rating);
            }
            catch (ArgumentException ex)
            {
                starRatingResponseModel.ErrorMessage = ex.Message;
                return starRatingResponseModel;
            }
            finally
            {
                starRatingResponseModel.StarRatingsSum = await this.ratingsService.GetStarRatingsAsync(input.ProductId);
                starRatingResponseModel.NextVoteDate = await this.ratingsService.GetNextVoteDateAsync(input.ProductId, userId);
            }

            return starRatingResponseModel;
        }
    }
}
