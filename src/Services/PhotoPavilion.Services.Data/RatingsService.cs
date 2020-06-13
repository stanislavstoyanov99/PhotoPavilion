namespace PhotoPavilion.Services.Data
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;

    public class RatingsService : IRatingsService
    {
        private readonly IDeletableEntityRepository<StarRating> starRatingsRepository;

        public RatingsService(IDeletableEntityRepository<StarRating> starRatingsRepository)
        {
            this.starRatingsRepository = starRatingsRepository;
        }

        public async Task<int> GetStarRatingsAsync(int productId)
        {
            var starRatings = await this.starRatingsRepository
                .All()
                .Where(x => x.ProductId == productId)
                .SumAsync(x => x.Rate);

            return starRatings;
        }

        public async Task<DateTime> GetNextVoteDateAsync(int productId, string userId)
        {
            var starRating = await this.starRatingsRepository
                .All()
                .FirstAsync(x => x.ProductId == productId && x.UserId == userId);

            return starRating.NextVoteDate;
        }

        public async Task VoteAsync(int productId, string userId, int rating)
        {
            var starRating = await this.starRatingsRepository
                .All()
                .FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == userId);

            if (starRating != null)
            {
                if (DateTime.UtcNow < starRating.NextVoteDate)
                {
                    throw new ArgumentException(ExceptionMessages.AlreadySentVote);
                }

                starRating.Rate += rating;
                starRating.NextVoteDate = DateTime.UtcNow.AddDays(1);
            }
            else
            {
                starRating = new StarRating
                {
                    ProductId = productId,
                    UserId = userId,
                    Rate = rating,
                    NextVoteDate = DateTime.UtcNow.AddDays(1),
                };

                await this.starRatingsRepository.AddAsync(starRating);
            }

            await this.starRatingsRepository.SaveChangesAsync();
        }
    }
}
