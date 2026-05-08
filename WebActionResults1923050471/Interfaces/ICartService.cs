using WebActionResults1923050471.Models;

namespace WebActionResults1923050471.Interfaces
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(HttpContext httpContext, int accountId);
        Task AddToCartAsync(HttpContext httpContext, int accountId, CartItem item);
        Task RemoveFromCartAsync(HttpContext httpContext, int accountId, int productId);
        Task UpdateCartAsync(HttpContext httpContext, int accountId, Cart cart);
        Task ClearCartAsync(HttpContext httpContext, int accountId);
        Task AddItemAsync(Cart cart, CartItem item);
        Task RemoveItemAsync(Cart cart, int productId);
        Task UpdateQuantityAsync(Cart cart, int productId, int quantity);
        Task ClearAsync(Cart cart);
    }
}
