using WebActionResults1923050471.Models;

namespace WebActionResults1923050471.Interfaces
{
    public interface ICartService
    {
        ShoppingCart GetCart(HttpContext httpContext, int accountId);
        void AddToCart(HttpContext httpContext, int accountId, CartItem item);
        void RemoveFromCart(HttpContext httpContext, int accountId, int productId);
        void UpdateCart(HttpContext httpContext, int accountId, ShoppingCart cart);
        void ClearCart(HttpContext httpContext, int accountId);
    }
}
