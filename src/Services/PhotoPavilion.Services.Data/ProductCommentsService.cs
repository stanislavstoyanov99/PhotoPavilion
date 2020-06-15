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

    public class ProductCommentsService : IProductCommentsService
    {
        private readonly IDeletableEntityRepository<Comment> commentsRepository;

        public ProductCommentsService(IDeletableEntityRepository<Comment> commentsRepository)
        {
            this.commentsRepository = commentsRepository;
        }

        public async Task CreateAsync(int productId, string userId, string content, int? parentId = null)
        {
            var productComment = new Comment
            {
                ProductId = productId,
                UserId = userId,
                Content = content,
                ParentId = parentId,
            };

            bool doesProductCommentExist = await this.commentsRepository
                .All()
                .AnyAsync(x => x.ProductId == productComment.ProductId && x.UserId == userId && x.Content == content);
            if (doesProductCommentExist)
            {
                throw new ArgumentException(
                    string.Format(ExceptionMessages.ProductCommentAlreadyExists, productComment.ProductId, productComment.Content));
            }

            await this.commentsRepository.AddAsync(productComment);
            await this.commentsRepository.SaveChangesAsync();
        }

        public async Task<bool> IsInProductId(int commentId, int productId)
        {
            var commentProductId = await this.commentsRepository
                .All()
                .Where(x => x.Id == commentId)
                .Select(x => x.ProductId)
                .FirstOrDefaultAsync();

            return commentProductId == productId;
        }
    }
}
