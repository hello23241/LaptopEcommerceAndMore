using Microsoft.AspNetCore.Mvc;
using LaptopEcommerceAndMore.Interfaces;

namespace LaptopEcommerceAndMore.Controllers
{
    public class DebugController(IHeaderBadgeService headerBadgeService) : Controller
    {
        private readonly IHeaderBadgeService _headerBadgeService = headerBadgeService;

        [HttpPost]
        public async Task<IActionResult> IncrementWishlist()
        {
            await _headerBadgeService.IncrementWishlistCountAsync();
            return RedirectToRefererOrHome();
        }

        [HttpPost]
        public async Task<IActionResult> DecrementWishlist()
        {
            await _headerBadgeService.DecrementWishlistCountAsync();
            return RedirectToRefererOrHome();
        }

        [HttpPost]
        public async Task<IActionResult> IncrementCart()
        {
            await _headerBadgeService.IncrementCartCountAsync();
            return RedirectToRefererOrHome();
        }

        [HttpPost]
        public async Task<IActionResult> DecrementCart()
        {
            await _headerBadgeService.DecrementCartCountAsync();
            return RedirectToRefererOrHome();
        }

        private IActionResult RedirectToRefererOrHome()
        {
            var referer = Request.Headers.Referer.ToString();
            if (!string.IsNullOrWhiteSpace(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Index", "Product");
        }
    }
}

