using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LaptopEcommerceAndMore.Data;
using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.Models;
using LaptopEcommerceAndMore.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LaptopEcommerceAndMore.Pages
{
    public class CheckoutModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncryptionService _encryptionService;
        private readonly ICartService _cartService;
        private readonly ICurrencyService _currencyService;

        public CheckoutModel(ApplicationDbContext context, IEncryptionService encryptionService, ICartService cartService, ICurrencyService currencyService)
        {
            _context = context;
            _encryptionService = encryptionService;
            _cartService = cartService;
            _currencyService = currencyService;
        }

        public ShoppingCart CurrentCart { get; set; }
        public string DisplayFullName { get; set; }
        public string DisplayEmail { get; set; }
        public string MaskedPhone { get; set; }
        public string MaskedAddress { get; set; }

        [BindProperty]
        public decimal UsdToVndRate { get; set; } = 25400m;

        // Bảng tra cứu phí Ship theo CountryCode (Đơn vị mặc định: USD gốc)
        public Dictionary<string, decimal> ShippingRates => new Dictionary<string, decimal>
        {
            { "VN", 2.00m },
            { "SG", 15.00m },
            { "JP", 25.00m },
            { "US", 50.00m },
            { "FR", 45.00m }
        };

        // --- HÀM TẢI TRANG BAN ĐẦU ---
        public async Task<IActionResult> OnGetAsync()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return RedirectToPage("/Account/Login", new { returnUrl = "/Checkout" });
            }

            try { UsdToVndRate = await _currencyService.GetUsdToVndRateAsync(); } catch { }

            // Lấy giỏ hàng thực tế của UserId này để kiểm tra xem có hàng không
            CurrentCart = await _cartService.GetCartAsync(HttpContext, userId);
            if (CurrentCart == null || !CurrentCart.Items.Any()) return RedirectToPage("/Cart");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null)
            {
                DisplayFullName = user.FullName;
                DisplayEmail = user.Email;
                MaskedPhone = ApplyPhoneMasking(_encryptionService.Decrypt(user.PhoneNumber));
                MaskedAddress = ApplyAddressMasking(_encryptionService.Decrypt(user.Address));
            }

            return Page();
        }

        // --- API LẤY PHÍ SHIPPING GỬI VỀ CHO AJAX FRONTEND ---
        public IActionResult OnGetShippingCost(string countryCode)
        {
            if (ShippingRates.TryGetValue(countryCode, out decimal rate))
            {
                return new JsonResult(new { success = true, cost = rate });
            }
            return new JsonResult(new { success = true, cost = 35.00m }); // Giá dự phòng cho quốc gia khác
        }

        // --- LUỒNG XỬ LÝ CHÍNH: BẤM ĐẶT HÀNG (DÀNH CHO CẢ COD VÀ VIETQR) ---
        public async Task<IActionResult> OnPostPlaceOrderAsync(string countryCode, string selectedPayment)
        {
            // Bước 1: Khóa bảo mật - Định danh chính xác UserId đang cầm Session Cart
            var userIdString = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return new JsonResult(new { success = false, message = "Unauthorized Session" });
            }

            // Bước 2: Lấy giỏ hàng thực tế của chính User đó từ Database/Service
            var userCart = await _cartService.GetCartAsync(HttpContext, userId);
            if (userCart == null || !userCart.Items.Any())
            {
                return new JsonResult(new { success = false, message = "Your shopping cart is empty." });
            }

            // Bước 3: Tính toán toán học tổng chi phí gốc (USD) tại Backend để chống sửa giá ở Client
            if (string.IsNullOrEmpty(countryCode) || !ShippingRates.TryGetValue(countryCode, out decimal shippingCost))
            {
                shippingCost = 35.00m;
            }
            decimal finalTotalUsd = userCart.TotalPrice + shippingCost;

            // Bước 4: Giải mã địa chỉ lưu trữ thật từ tài khoản User để đưa vào hóa đơn giao hàng
            string actualShippingAddress = "No Address Provided";
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null && !string.IsNullOrEmpty(user.Address))
            {
                actualShippingAddress = _encryptionService.Decrypt(user.Address);
            }

            // ================= LUỒNG PHÂN NHÁNH PHƯƠNG THỨC =================

            // PHÂN NHÁNH A: KHÁCH CHỌN CHUYỂN KHOẢN NGÂN HÀNG (VIETQR)
            if (selectedPayment == "VietQR")
            {
                try { UsdToVndRate = await _currencyService.GetUsdToVndRateAsync(); } catch { }
                int finalTotalVnd = (int)Math.Floor(finalTotalUsd * UsdToVndRate);

                string yourBank = "TCB"; // Ngân hàng Techcombank của bạn
                string yourAccountNo = "19076216865011"; // Số tài khoản xịn của bạn
                string description = $"DH{userId}";

                // Ghi nhận đơn hàng thật vào Database với trạng thái Chờ chuyển khoản
                int generatedOrderId = await InsertOrderToDatabase(userId, finalTotalUsd, "VietQR", "Awaiting Transfer", actualShippingAddress, userCart);

                // Giải phóng sạch sẽ giỏ hàng ngay sau khi đơn hàng đã được lập thành công
                await _cartService.ClearCartAsync(HttpContext, userId);

                string safeDescription = Uri.EscapeDataString(description);
                string vietQrImageUrl = $"https://img.vietqr.io/image/{yourBank}-{yourAccountNo}-compact.png?amount={finalTotalVnd}&addInfo={safeDescription}";

                // Trả kết quả về cho AJAX mở Modal đếm ngược và kiểm tra trạng thái tại chỗ
                return new JsonResult(new
                {
                    success = true,
                    isVietQR = true,
                    qrUrl = vietQrImageUrl,
                    orderId = generatedOrderId,
                    amountVnd = finalTotalVnd,
                    content = description
                });
            }

            // PHÂN NHÁNH B: KHÁCH CHỌN THANH TOÁN KHI NHẬN HÀNG (COD)
            // Chỉ đơn giản là đưa lên database, ghi nhận trạng thái chờ xử lý giao hàng, chưa cần trả tiền liền.
            await InsertOrderToDatabase(userId, finalTotalUsd, "COD", "Pending Delivery", actualShippingAddress, userCart);
            await _cartService.ClearCartAsync(HttpContext, userId);

            // Báo thành công về để Front-end chuyển hướng người dùng sang trang chủ kèm Banner ăn mừng
            return new JsonResult(new { success = true, isVietQR = false, redirectUrl = "/Index?status=success_cod" });
        }

        // --- LUỒNG XỬ LÝ PHỤ: KHI PAYPAL FRONTEND ĐÃ TRỪ TIỀN THÀNH CÔNG ---
        public async Task<IActionResult> OnGetPayPalSuccess(string countryCode, decimal totalUsd, string payPalOrderId)
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            if (!int.TryParse(userIdString, out int userId)) return RedirectToPage("/Account/Login");

            var userCart = await _cartService.GetCartAsync(HttpContext, userId);

            string actualShippingAddress = "No Address Provided";
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user != null && !string.IsNullOrEmpty(user.Address))
            {
                actualShippingAddress = _encryptionService.Decrypt(user.Address);
            }

            await InsertOrderToDatabase(userId, totalUsd, $"PayPal ({payPalOrderId})", "Paid / Processing", actualShippingAddress, userCart);
            await _cartService.ClearCartAsync(HttpContext, userId);

            return RedirectToPage("/Index", new { status = "success_paypal" });
        }

        // --- API KIỂM TRA TRẠNG THÁI GIAO DỊCH DÀNH CHO MODAL VIETQR QUÉT NGẦM ---
        public async Task<IActionResult> OnGetCheckOrderStatusAsync(int orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderID == orderId);
            if (order != null)
            {
                return new JsonResult(new { success = true, status = order.Status });
            }
            return new JsonResult(new { success = false });
        }

        // ================= HÀM THỰC THI CORE: ĐẨY DỮ LIỆU LÊN DATABASE SQL SERVER SUỐT LUỒNG =================
        private async Task<int> InsertOrderToDatabase(int userId, decimal totalCost, string method, string status, string address, ShoppingCart cart)
        {
            // 1. Khởi tạo thực thể cho bảng cha Orders
            var newOrder = new Orders
            {
                UserID = userId,
                TotalAmount = totalCost, // Tổng tiền đã cộng phí Ship
                PaymentMethod = method,   // COD, PayPal hoặc VietQR
                Status = status,         // Trạng thái xử lý
                ShippingAddress = address, // Địa chỉ đã được giải mã thô
                OrderDate = DateTime.UtcNow   // Thời gian chốt đơn
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync(); // Lệnh Save 1: Đẩy dữ liệu xuống để SQL Server cấp phát ID tự động cho OrderID

            // 2. Vòng lặp bóc tách từng sản phẩm trong Giỏ hàng đẩy vào bảng con OrderDetails
            foreach (var item in cart.Items)
            {
                var detail = new OrderDetails
                {
                    OrderID = newOrder.OrderID,      // Gán ID khóa ngoại liên kết chính xác từ bảng cha vừa tạo ở trên
                    ProductID = item.ProductId,      // Đảm bảo lấy đúng ProductID trong giỏ hàng
                    Quantity = item.Quantity,        // Đúng số lượng khách đặt
                    UnitPrice = item.Price           // Giá gốc của sản phẩm tại thời điểm mua chốt đơn
                };
                _context.OrderDetails.Add(detail);
            }

            await _context.SaveChangesAsync(); // Lệnh Save 2: Đẩy toàn bộ danh sách sản phẩm con lên Azure SQL Server
            return newOrder.OrderID; // Trả về mã số ID đơn hàng để phục vụ luồng check VietQR
        }

        private string ApplyPhoneMasking(string p) => string.IsNullOrEmpty(p) ? "" : p.Substring(0, 3) + "******" + p.Substring(p.Length - 3);
        private string ApplyAddressMasking(string a) => string.IsNullOrEmpty(a) ? "" : a.Substring(0, 6) + "******" + a.Substring(a.Length - 6);
    }
}