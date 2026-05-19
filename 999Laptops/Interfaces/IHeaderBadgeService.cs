using System.Threading.Tasks;

namespace LaptopEcommerceAndMore.Interfaces
{
    public interface IHeaderBadgeService
    {
        Task<int> GetWishlistCountAsync(int userId);
        Task<int> GetCartCountAsync(int userId);
    }
}