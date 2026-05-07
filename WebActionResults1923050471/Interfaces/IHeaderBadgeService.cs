namespace WebActionResults1923050471.Interfaces
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
