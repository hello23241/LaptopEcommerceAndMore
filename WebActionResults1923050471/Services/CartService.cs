using WebActionResults1923050471.Models;
using System.Text.Json;

namespace WebActionResults1923050471.Services
{
    public interface ICartService
    {
        ShoppingCart GetCart(HttpContext httpContext, int accountId);
        void AddToCart(HttpContext httpContext, int accountId, CartItem item);
        void RemoveFromCart(HttpContext httpContext, int accountId, int productId);
        void UpdateCart(HttpContext httpContext, int accountId, ShoppingCart cart);
        void ClearCart(HttpContext httpContext, int accountId);
    }

    public class CartService : ICartService
    {
        private const string CartSessionKey = "ShoppingCart_";

        public ShoppingCart GetCart(HttpContext httpContext, int accountId)
        {
            var key = CartSessionKey + accountId;
            var session = httpContext.Session;
            var cartJson = session.GetString(key);

            if (string.IsNullOrEmpty(cartJson))
            {
                return new ShoppingCart { AccountId = accountId };
            }

            return JsonSerializer.Deserialize<ShoppingCart>(cartJson) ?? new ShoppingCart { AccountId = accountId };
        }

        public void AddToCart(HttpContext httpContext, int accountId, CartItem item)
        {
            var cart = GetCart(httpContext, accountId);
            cart.AddItem(item);
            UpdateCart(httpContext, accountId, cart);
        }

        public void RemoveFromCart(HttpContext httpContext, int accountId, int productId)
        {
            var cart = GetCart(httpContext, accountId);
            cart.RemoveItem(productId);
            UpdateCart(httpContext, accountId, cart);
        }

        public void UpdateCart(HttpContext httpContext, int accountId, ShoppingCart cart)
        {
            var key = CartSessionKey + accountId;
            var session = httpContext.Session;
            var cartJson = JsonSerializer.Serialize(cart);
            session.SetString(key, cartJson);
        }

        public void ClearCart(HttpContext httpContext, int accountId)
        {
            var key = CartSessionKey + accountId;
            var session = httpContext.Session;
            session.Remove(key);
        }
    }
}
