namespace LaptopEcommerceAndMore.Interfaces
{
    public interface IHeaderBadgeService
    {
        Task<int> GetWishlistCountAsync();
        Task<int> GetCartCountAsync();
        Task IncrementWishlistCountAsync();
        Task IncrementCartCountAsync();
        Task DecrementWishlistCountAsync();
        Task DecrementCartCountAsync();
    }
}

