using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace LaptopEcommerceAndMore.Pages
{
    [AllowAnonymous]
    public class CartModel : PageModel
    {
        private readonly ICartService _cartService;

        public CartModel(ICartService cartService)
        {
            _cartService = cartService;
        }

        public ShoppingCart Cart { get; set; }

        public async Task OnGetAsync()
        {
            await LoadCart();
        }

        public async Task<IActionResult> OnPostUpdateQuantityAsync(int productId, int quantity)
        {
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int productId)
        {
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

            Cart = await _cartService.GetCartAsync(HttpContext, currentUserId);
        }
    }
}
