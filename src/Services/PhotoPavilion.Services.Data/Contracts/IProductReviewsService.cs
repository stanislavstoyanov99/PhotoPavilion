namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface IProductReviewsService
    {
        Task CreateAsync(int productId, string userId, string title, string description);
    }
}
