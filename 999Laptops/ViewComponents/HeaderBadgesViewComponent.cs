using Microsoft.AspNetCore.Mvc;
using LaptopEcommerceAndMore.Interfaces;

namespace LaptopEcommerceAndMore.ViewComponents
{
    public class HeaderBadgesViewComponent(IHeaderBadgeService headerBadgeService) : ViewComponent
    {
        private readonly IHeaderBadgeService _headerBadgeService = headerBadgeService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy UserId từ Session
            var userId = HttpContext.Session.GetString("UserId");
            var wishlistCount = await _headerBadgeService.GetWishlistCountAsync();
            var cartCount = await _headerBadgeService.GetCartCountAsync();

            var model = new HeaderBadgesViewModel(wishlistCount, cartCount);
            return View(model);
        }
    }

        public record HeaderBadgesViewModel(int WishlistCount, int CartCount);
}

