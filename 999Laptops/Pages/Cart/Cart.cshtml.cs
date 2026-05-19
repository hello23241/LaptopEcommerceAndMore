using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LaptopEcommerceAndMore.Data;
using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LaptopEcommerceAndMore.Pages
{
    // Không dùng thuộc tính [AllowAnonymous] của MVC ở đây nữa để tránh xung đột định tuyến
    public class CartModel : PageModel
    {
        private readonly ICartService _cartService;
        private readonly ApplicationDbContext _context;

        public CartModel(ICartService cartService, ApplicationDbContext context)
        {
            _cartService = cartService;
            _context = context;
        }

        public ShoppingCart Cart { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCart();
            return Page();
        }

        public async Task<IActionResult> OnPostUpdateQuantityAsync(int productId, int quantity)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return RedirectToPage();
            }

            var cartItem = await _context.Cart
                .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == productId);

            if (cartItem == null)
            {
                return RedirectToPage();
            }

            if (quantity <= 0)
            {
                _context.Cart.Remove(cartItem);
            }
            else
            {
                cartItem.Quantity = quantity;
                _context.Cart.Update(cartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int productId)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out var userId))
            {
                return RedirectToPage();
            }

            var cartItem = await _context.Cart
                .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == productId);

            if (cartItem != null)
            {
                _context.Cart.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCheckoutAsync()
        {
            return RedirectToPage("/Checkout");
        }

        private async Task LoadCart()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            int currentUserId = 0;

            if (!string.IsNullOrEmpty(userIdString))
            {
                int.TryParse(userIdString, out currentUserId);
            }

            // Hàm này sẽ tự trả về giỏ hàng trống nếu currentUserId == 0 mà không crash/redirect
            Cart = await _cartService.GetCartAsync(HttpContext, currentUserId);
        }
    }
}