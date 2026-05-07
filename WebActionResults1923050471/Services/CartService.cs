using WebActionResults1923050471.Models;
using WebActionResults1923050471.Interfaces;
using System.Text.Json;

namespace WebActionResults1923050471.Services
{
    public class CartService : ICartService
    {
        private const string CartSessionKey = "ShoppingCart_";

        public Task<ShoppingCart> GetCartAsync(HttpContext httpContext, int accountId)
        {
            var key = CartSessionKey + accountId;
            var session = httpContext.Session;
            var cartJson = session.GetString(key);

            if (string.IsNullOrEmpty(cartJson))
            {
                return Task.FromResult(new ShoppingCart { AccountId = accountId, TotalItems = 0, TotalPrice = 0m });
            }

            var cart = JsonSerializer.Deserialize<ShoppingCart>(cartJson) ?? new ShoppingCart { AccountId = accountId, TotalItems = 0, TotalPrice = 0m };
            cart.TotalItems = cart.Items.Sum(x => x.Quantity);
            cart.TotalPrice = cart.Items.Sum(x => x.Subtotal);
            return Task.FromResult(cart);
        }

        public async Task AddToCartAsync(HttpContext httpContext, int accountId, CartItem item)
        {
            var cart = await GetCartAsync(httpContext, accountId);
            await AddItemAsync(cart, item);
            await UpdateCartAsync(httpContext, accountId, cart);
        }

        public async Task RemoveFromCartAsync(HttpContext httpContext, int accountId, int productId)
        {
            var cart = await GetCartAsync(httpContext, accountId);
            await RemoveItemAsync(cart, productId);
            await UpdateCartAsync(httpContext, accountId, cart);
        }

        public Task UpdateCartAsync(HttpContext httpContext, int accountId, ShoppingCart cart)
        {
            var key = CartSessionKey + accountId;
            var session = httpContext.Session;
            var cartJson = JsonSerializer.Serialize(cart);
            session.SetString(key, cartJson);
            return Task.CompletedTask;
        }

        public Task ClearCartAsync(HttpContext httpContext, int accountId)
        {
            var key = CartSessionKey + accountId;
            var session = httpContext.Session;
            session.Remove(key);
            return Task.CompletedTask;
        }

        public Task AddItemAsync(ShoppingCart cart, CartItem item)
        {
            item.Subtotal = item.Price * item.Quantity;
            var existingItem = cart.Items.FirstOrDefault(x => x.ProductId == item.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += item.Quantity;
                existingItem.Subtotal = existingItem.Price * existingItem.Quantity;
            }
            else
            {
                cart.Items.Add(item);
            }

            cart.TotalItems = cart.Items.Sum(x => x.Quantity);
            cart.TotalPrice = cart.Items.Sum(x => x.Subtotal);

            return Task.CompletedTask;
        }

        public Task RemoveItemAsync(ShoppingCart cart, int productId)
        {
            cart.Items.RemoveAll(x => x.ProductId == productId);
            cart.TotalItems = cart.Items.Sum(x => x.Quantity);
            cart.TotalPrice = cart.Items.Sum(x => x.Subtotal);
            return Task.CompletedTask;
        }

        public Task UpdateQuantityAsync(ShoppingCart cart, int productId, int quantity)
        {
            var item = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (item != null)
            {
                if (quantity <= 0)
                {
                    cart.Items.RemoveAll(x => x.ProductId == productId);
                }
                else
                {
                    item.Quantity = quantity;
                    item.Subtotal = item.Price * item.Quantity;
                }
            }

            cart.TotalItems = cart.Items.Sum(x => x.Quantity);
            cart.TotalPrice = cart.Items.Sum(x => x.Subtotal);

            return Task.CompletedTask;
        }

        public Task ClearAsync(ShoppingCart cart)
        {
            cart.Items.Clear();
            cart.TotalItems = 0;
            cart.TotalPrice = 0m;
            return Task.CompletedTask;
        }
    }
}
