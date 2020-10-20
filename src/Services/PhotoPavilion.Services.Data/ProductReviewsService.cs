namespace PhotoPavilion.Services.Data
{
    using System;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;

    using PhotoPavilion.Data.Common.Repositories;
    using PhotoPavilion.Data.Models;
    using PhotoPavilion.Services.Data.Common;
    using PhotoPavilion.Services.Data.Contracts;

    public class ProductReviewsService : IProductReviewsService
    {
        private readonly IDeletableEntityRepository<Review> reviewsRepository;

        public ProductReviewsService(IDeletableEntityRepository<Review> reviewsRepository)
        {
            this.reviewsRepository = reviewsRepository;
        }

        public async Task CreateAsync(int productId, string userId, string title, string description)
        {
            var productReview = new Review
            {
                ProductId = productId,
                UserId = userId,
                Title = title,
                Description = description,
            };

            bool doesProductReviewExist = await this.reviewsRepository
                .All()
                .AnyAsync(x => x.ProductId == productReview.ProductId &&
                    x.UserId == userId &&
                    x.Title == title &&
                    x.Description == description);
            if (doesProductReviewExist)
            {
                throw new ArgumentException(
                    string.Format(ExceptionMessages.ProductReviewAlreadyExists, productReview.ProductId, productReview.Title, productReview.Description));
            }

            await this.reviewsRepository.AddAsync(productReview);
            await this.reviewsRepository.SaveChangesAsync();
        }
    }
}
