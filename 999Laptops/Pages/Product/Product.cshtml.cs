using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaptopEcommerceAndMore.Pages.Product
{
    public class ProductDetailModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly Data.ApplicationDbContext _context;
        private readonly ICurrencyService _currencyService;

        public ProductDetailModel(IDataService dataService, Data.ApplicationDbContext context, ICurrencyService currencyService)
        {
            _dataService = dataService;
            _context = context;
            _currencyService = currencyService;
        }

        public Products SingleProduct { get; set; }
        public decimal UsdToVndRate { get; set; } = 25000m;

        // ĐÃ THÊM: Danh sách chứa các review của sản phẩm này để hiển thị ra giao diện
        public List<Reviews> ProductReviews { get; set; } = new List<Reviews>();
        public string CurrentUsername { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (id <= 0) return NotFound();

            SingleProduct = await _dataService.GetProductByIdAsync(id);
            if (SingleProduct == null) return NotFound();

            // ĐÃ THÊM: Lấy Username từ Session nếu người dùng đã đăng nhập để điền vào form cứng
            var userName = HttpContext.Session.GetString("UserName");
            CurrentUsername = string.IsNullOrWhiteSpace(userName) ? "Guest" : userName;

            // ĐÃ THÊM: Tải danh sách Review kèm thông tin User từ Database
            ProductReviews = await _context.Reviews
                .Include(r => r.User) // Đảm bảo thực thể Review có liên kết với bảng User để lấy tên
                .Where(r => r.ProductID == id)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();

            try
            {
                UsdToVndRate = await _currencyService.GetUsdToVndRateAsync();
            }
            catch
            {
                // Giữ nguyên mặc định nếu lỗi mạng
            }

            return Page();
        }

        // --- HANDLER: XỬ LÝ GỬI ĐÁNH GIÁ MỚI ---
        public async Task<IActionResult> OnPostAddReviewAsync(int productId, int rating, string comment)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Redirect($"/ProductDetail/{productId}");
            }

            if (string.IsNullOrEmpty(comment))
            {
                ModelState.AddModelError(string.Empty, "Comment is required.");
                return Redirect($"/ProductDetail/{productId}");
            }

            // KIỂM TRA ĐÃ MUA HÀNG (IsPurchased): Quét bảng Orders xem User này từng có đơn hoàn thành chứa sản phẩm này chưa
            // Tùy thuộc vào cấu trúc bảng Orders của bạn, dưới đây là logic chuẩn kiểm tra tổng quan:
            bool hasPurchased = await _context.OrderDetails
                .Include(od => od.Order)
                .AnyAsync(od => od.Order.UserID == userId && od.ProductID == productId);

            var newReview = new Reviews
            {
                UserID = userId,
                ProductID = productId,
                Rating = rating < 1 ? 1 : (rating > 5 ? 5 : rating),
                Comment = comment,
                ReviewDate = DateTime.UtcNow,
                IsPurchased = hasPurchased
            };

            _context.Reviews.Add(newReview);
            await _context.SaveChangesAsync();

            return Redirect($"/ProductDetail/{productId}#product-tab");
        }

        // --- HANDLER: THÊM VÀO GIỎ HÀNG ---
        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Redirect($"/ProductDetail/{productId}");
            }

            if (quantity <= 0) quantity = 1;

            var existingCartItem = await _context.Cart
                .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == productId);

            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                _context.Cart.Update(existingCartItem);
            }
            else
            {
                var newCart = new LaptopEcommerceAndMore.Models.Cart
                {
                    UserID = userId,
                    ProductID = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.UtcNow
                };
                _context.Cart.Add(newCart);
            }

            await _context.SaveChangesAsync();
            return Redirect($"/ProductDetail/{productId}");
        }

        // --- HANDLER: THÊM VÀO WISHLIST ---
        public async Task<IActionResult> OnPostAddToWishlistAsync(int productId)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Redirect($"/ProductDetail/{productId}");
            }

            var existingWish = await _context.Wishlist
                .FirstOrDefaultAsync(w => w.UserID == userId && w.ProductID == productId);

            if (existingWish == null)
            {
                var newWish = new Wishlist
                {
                    UserID = userId,
                    ProductID = productId,
                    AddedAt = DateTime.UtcNow
                };
                _context.Wishlist.Add(newWish);
                await _context.SaveChangesAsync();
            }

            return Redirect($"/ProductDetail/{productId}");
        }
    }
}