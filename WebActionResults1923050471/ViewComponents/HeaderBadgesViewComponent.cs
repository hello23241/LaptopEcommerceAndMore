using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.ViewComponents
{
    public class HeaderBadgesViewComponent(IHeaderBadgeService headerBadgeService) : ViewComponent
    {
        private readonly IHeaderBadgeService _headerBadgeService = headerBadgeService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var wishlistCount = await _headerBadgeService.GetWishlistCountAsync();
            var cartCount = await _headerBadgeService.GetCartCountAsync();
            var model = new HeaderBadgesViewModel(wishlistCount, cartCount);
            return View(model);
        }
    }

    public record HeaderBadgesViewModel(int WishlistCount, int CartCount);
}
