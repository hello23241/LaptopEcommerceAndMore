using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.Data; // Thêm namespace chứa DbContext của bạn
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace LaptopEcommerceAndMore.Services
{
    public class HeaderBadgeService : IHeaderBadgeService
    {
        private readonly ApplicationDbContext _context;

        public HeaderBadgeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Đếm số loại sản phẩm trong mục yêu thích (ví dụ: thích 3 máy laptop khác nhau -> hiện số 3)
        public async Task<int> GetWishlistCountAsync(int userId)
        {
            return await _context.Wishlist
                .Where(w => w.UserID == userId)
                .CountAsync();
        }

        // Tính tổng số lượng của tất cả vật phẩm trong giỏ (ví dụ: mua 2 máy Dell, 1 máy HP -> hiện số 3)
        public async Task<int> GetCartCountAsync(int userId)
        {
            return await _context.Cart
                .Where(c => c.UserID == userId)
                .SumAsync(c => (int?)c.Quantity) ?? 0; // Tránh lỗi nếu giỏ hàng trống trơn
        }
    }
}