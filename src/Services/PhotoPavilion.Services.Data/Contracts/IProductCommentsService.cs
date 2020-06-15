namespace PhotoPavilion.Services.Data.Contracts
{
    using System.Threading.Tasks;

    public interface IProductCommentsService
    {
        Task CreateAsync(int productId, string userId, string content, int? parentId = null);

        Task<bool> IsInProductId(int commentId, int productId);
    }
}
