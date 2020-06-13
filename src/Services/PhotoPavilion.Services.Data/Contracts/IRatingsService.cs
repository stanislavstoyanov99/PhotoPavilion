namespace PhotoPavilion.Services.Data.Contracts
{
    using System;
    using System.Threading.Tasks;

    public interface IRatingsService
    {
        Task VoteAsync(int productId, string userId, int rating);

        Task<int> GetStarRatingsAsync(int productId);

        Task<DateTime> GetNextVoteDateAsync(int productId, string userId);
    }
}
