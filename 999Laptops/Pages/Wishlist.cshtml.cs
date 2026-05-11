using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LaptopEcommerceAndMore.Models;

namespace LaptopEcommerceAndMore.Pages
{
    public class WishlistModel : PageModel
    {
        // Hi?n t?i d? d? li?u tr?ng d? trang không b? crash
        public List<Products> WishlistItems { get; set; } = new List<Products>();

        public void OnGet()
        {
            // Sau này b?n s? g?i Service d? l?y d? li?u th?t ? dây
        }

        public async Task<IActionResult> OnPostMoveToCartAsync(int id)
        {
            // Logic x? lý khi nh?n nút Move to Cart
            return RedirectToPage();
        }
    }
}
