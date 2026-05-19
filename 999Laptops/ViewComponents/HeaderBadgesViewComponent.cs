using Microsoft.AspNetCore.Mvc;
using LaptopEcommerceAndMore.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LaptopEcommerceAndMore.ViewComponents
{
    public class HeaderBadgesViewComponent : ViewComponent
    {
        private readonly IHeaderBadgeService _headerBadgeService;

        public HeaderBadgesViewComponent(IHeaderBadgeService headerBadgeService)
        {
            _headerBadgeService = headerBadgeService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // 1. Kiểm tra trạng thái đăng nhập từ Session của Layout
            var userIdString = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                // Nếu CHƯA đăng nhập -> Ép số lượng về 0 để ẩn hoàn toàn các badge đỏ
                var emptyModel = new HeaderBadgesViewModel(0, 0);
                return View(emptyModel);
            }

            // 2. Nếu ĐÃ đăng nhập -> Gọi Service tính toán real-time từ Database dựa trên UserId
            var wishlistCount = await _headerBadgeService.GetWishlistCountAsync(userId);
            var cartCount = await _headerBadgeService.GetCartCountAsync(userId);

            var model = new HeaderBadgesViewModel(wishlistCount, cartCount);
            return View(model);
        }

        public record HeaderBadgesViewModel(int WishlistCount, int CartCount);
    }
}