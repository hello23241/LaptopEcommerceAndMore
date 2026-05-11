using Microsoft.AspNetCore.Mvc;
using LaptopEcommerceAndMore.Models;
using LaptopEcommerceAndMore.Interfaces;

namespace LaptopEcommerceAndMore.Controllers
{
    public class CartController(IDataService dataService, ICartService cartService) : Controller
    {
        private readonly IDataService _dataService = dataService;
        private readonly ICartService _cartService = cartService;

        private int GetUserId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not logged in");
            }
            return userId.Value;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.GetCartAsync(HttpContext, userId);
                return View(cart);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var product = await _dataService.GetProductByIdAsync(productId);
                if (product == null)
                    return NotFound();

                var userId = GetUserId();
                var cartItem = new CartItem
                {
                    ProductId = product.ProductID,
                    ProductName = product.ProductName,
                    Price = product.BasePrice,
                    Quantity = quantity,
                    Subtotal = product.BasePrice * quantity
                };

                await _cartService.AddToCartAsync(HttpContext, userId, cartItem);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            try
            {
                var userId = GetUserId();
                await _cartService.RemoveFromCartAsync(HttpContext, userId, productId);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int productId, int quantity)
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.GetCartAsync(HttpContext, userId);
                await _cartService.UpdateQuantityAsync(cart, productId, quantity);
                await _cartService.UpdateCartAsync(HttpContext, userId, cart);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Checkout()
        {
            try
            {
                var userId = GetUserId();
                var cart = await _cartService.GetCartAsync(HttpContext, userId);
                
                if (!cart.Items.Any())
                {
                    ModelState.AddModelError("", "Cart is empty");
                    return RedirectToAction("Index");
                }

                // Here you would process the order
                await _cartService.ClearCartAsync(HttpContext, userId);
                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("Index", "Product");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public async Task<IActionResult> ClearCart()
        {
            try
            {
                var userId = GetUserId();
                await _cartService.ClearCartAsync(HttpContext, userId);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}

