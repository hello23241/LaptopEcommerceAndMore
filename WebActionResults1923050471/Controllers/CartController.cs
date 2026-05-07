using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Models;
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.Controllers
{
    public class CartController : Controller
    {
        private readonly IDataService _dataService;
        private readonly ICartService _cartService;

        public CartController(IDataService dataService, ICartService cartService)
        {
            _dataService = dataService;
            _cartService = cartService;
        }

        private int GetUserId()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not logged in");
            }
            return userId.Value;
        }

        public IActionResult Index()
        {
            try
            {
                var userId = GetUserId();
                var cart = _cartService.GetCart(HttpContext, userId);
                return View(cart);
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var product = _dataService.GetProductById(productId);
                if (product == null)
                    return NotFound();

                var userId = GetUserId();
                var cartItem = new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity
                };

                _cartService.AddToCart(HttpContext, userId, cartItem);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            try
            {
                var userId = GetUserId();
                _cartService.RemoveFromCart(HttpContext, userId, productId);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            try
            {
                var userId = GetUserId();
                var cart = _cartService.GetCart(HttpContext, userId);
                cart.UpdateQuantity(productId, quantity);
                _cartService.UpdateCart(HttpContext, userId, cart);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        [HttpPost]
        public IActionResult Checkout()
        {
            try
            {
                var userId = GetUserId();
                var cart = _cartService.GetCart(HttpContext, userId);
                
                if (!cart.Items.Any())
                {
                    ModelState.AddModelError("", "Cart is empty");
                    return RedirectToAction("Index");
                }

                // Here you would process the order
                _cartService.ClearCart(HttpContext, userId);
                TempData["SuccessMessage"] = "Order placed successfully!";
                return RedirectToAction("Index", "Product");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }

        public IActionResult ClearCart()
        {
            try
            {
                var userId = GetUserId();
                _cartService.ClearCart(HttpContext, userId);
                return RedirectToAction("Index");
            }
            catch
            {
                return RedirectToAction("Login", "Account");
            }
        }
    }
}
