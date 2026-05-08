using WebActionResults1923050471.Models;
using WebActionResults1923050471.ViewModels;

namespace WebActionResults1923050471.Interfaces
{
    public interface ICartService
    {
        Task<ShoppingCart> GetCartAsync(HttpContext httpContext, int accountId);
        Task AddToCartAsync(HttpContext httpContext, int accountId, CartItem item);
        Task RemoveFromCartAsync(HttpContext httpContext, int accountId, int productId);
        Task UpdateCartAsync(HttpContext httpContext, int accountId, ShoppingCart cart);
        Task ClearCartAsync(HttpContext httpContext, int accountId);
        Task AddItemAsync(ShoppingCart cart, CartItem item);
        Task RemoveItemAsync(ShoppingCart cart, int productId);
        Task UpdateQuantityAsync(ShoppingCart cart, int productId, int quantity);
        Task ClearAsync(ShoppingCart cart);
    }
}
