using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LaptopEcommerceAndMore.Data;
using LaptopEcommerceAndMore.Models;
using LaptopEcommerceAndMore.Interfaces; // ĐÃ THÊM: Để nhận diện ICurrencyService
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaptopEcommerceAndMore.Pages
{
    public class WishlistModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        // ĐÃ THÊM: Khai báo service tiền tệ trung gian
        private readonly ICurrencyService _currencyService;

        // ĐÃ SỬA: Inject thêm ICurrencyService vào Constructor
        public WishlistModel(ApplicationDbContext context, ICurrencyService currencyService)
        {
            _context = context;
            _currencyService = currencyService;
        }

        public List<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();

        public string CurrentSortBy { get; set; }
        public string CurrentOrder { get; set; }

        // ĐÃ THÊM: Thuộc tính chứa tỉ giá để trả ra ngoài giao diện hiển thị (.cshtml)
        public decimal UsdToVndRate { get; set; } = 25000m;

        public async Task<IActionResult> OnGetAsync(string sortBy = "date", string order = "desc")
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToPage("/Account/Login", new { returnUrl = "/Wishlist" });
            }

            CurrentSortBy = sortBy;
            CurrentOrder = order;

            // ĐÃ THÊM: Lấy tỷ giá thực tế ngầm từ API để nạp cho bộ đổi tiền VND
            try
            {
                UsdToVndRate = await _currencyService.GetUsdToVndRateAsync();
            }
            catch
            {
                // Giữ nguyên 25000m nếu API tiền tệ gặp sự cố rớt mạng
            }

            // Truy vấn lấy dữ liệu nạp kèm thông tin sản phẩm
            var query = _context.Wishlist
                .Include(w => w.Product)
                .Where(w => w.UserID == userId);

            // Xử lý sắp xếp logic
            if (sortBy == "alpha")
            {
                query = order == "asc"
                    ? query.OrderBy(w => w.Product.ProductName)
                    : query.OrderByDescending(w => w.Product.ProductName);
            }
            else
            {
                query = order == "asc"
                    ? query.OrderBy(w => w.AddedAt)
                    : query.OrderByDescending(w => w.AddedAt);
            }

            WishlistItems = await query.ToListAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostRemoveAsync(int id)
        {
            var wishItem = await _context.Wishlist.FindAsync(id);
            if (wishItem != null)
            {
                _context.Wishlist.Remove(wishItem);
                await _context.SaveChangesAsync();
            }
            return RedirectToPage(new { sortBy = HttpContext.Request.Query["sortBy"], order = HttpContext.Request.Query["order"] });
        }
    }
}