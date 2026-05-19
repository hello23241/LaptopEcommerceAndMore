using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LaptopEcommerceAndMore.Data;
using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.Models;
using System.Collections.Generic;
using System.Linq;

namespace LaptopEcommerceAndMore.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncryptionService _encryptionService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;

        public ProfileModel(ApplicationDbContext context, IEncryptionService encryptionService, IPasswordHasher passwordHasher, IEmailService emailService)
        {
            _context = context;
            _encryptionService = encryptionService;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
        }

        public Users CurrentUser { get; set; }
        public string DecryptedPhone { get; set; }
        public string DecryptedAddress { get; set; }
        public string MaskedPhone { get; set; }
        public string MaskedAddress { get; set; }
        public List<Orders> OrderHistory { get; set; } = new();

        [BindProperty]
        public EditProfileInput EditInput { get; set; }

        [BindProperty]
        public ChangePasswordInput ChangePassword { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            // 1. L?y UserId t? Session (luu ý ép ki?u ho?c ki?m tra null)
            var userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Account/Login");
            }

            // 2. Tìm ngu?i dùng trong Database
            // Gi? s? UserId trong DB là int, n?u là string hãy b? int.Parse
            int id = int.Parse(userIdStr);
            CurrentUser = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);

            if (CurrentUser == null)
            {
                return RedirectToPage("/Account/Login");
            }

            OrderHistory = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Where(o => o.UserID == id)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            // 3. GI?I MÃ
            try
            {
                DecryptedPhone = _encryptionService.Decrypt(CurrentUser.PhoneNumber);
                DecryptedAddress = _encryptionService.Decrypt(CurrentUser.Address);

                // T?o Masking cho Phone (ví d?: 090****747)
                if (DecryptedPhone.Length >= 7)
                    MaskedPhone = DecryptedPhone.Substring(0, 3) + "****" + DecryptedPhone.Substring(DecryptedPhone.Length - 3);
                else
                    MaskedPhone = "****";

                // T?o Masking cho Address (ví d?: 123 testing...)
                if (DecryptedAddress.Length > 10)
                    MaskedAddress = DecryptedAddress.Substring(0, 10) + "...";
                else
                    MaskedAddress = DecryptedAddress;
            }
            catch
            {
                DecryptedPhone = CurrentUser.PhoneNumber;
                DecryptedAddress = CurrentUser.Address;
                MaskedPhone = "********";
                MaskedAddress = "********";
            }

            EditInput = new EditProfileInput
            {
                FullName = CurrentUser.FullName,
                Email = CurrentUser.Email,
                Phone = DecryptedPhone,
                Address = DecryptedAddress
            };

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateProfileAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Account/Login");
            }

            if (EditInput == null)
            {
                TempData["ProfileError"] = "Invalid profile data.";
                return RedirectToPage();
            }

            if (string.IsNullOrWhiteSpace(EditInput.CurrentPassword))
            {
                TempData["ProfileError"] = "Please enter your password to update profile information.";
                return RedirectToPage();
            }

            int id = int.Parse(userIdStr);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Xác thực mật khẩu tại chỗ để chống Session Hijacking
            if (!_passwordHasher.Verify(EditInput.CurrentPassword, user.PasswordHash))
            {
                TempData["ProfileError"] = "The password you entered is incorrect.";
                return RedirectToPage();
            }

            string currentPhone = string.Empty;
            string currentAddress = string.Empty;
            try
            {
                currentPhone = _encryptionService.Decrypt(user.PhoneNumber);
                currentAddress = _encryptionService.Decrypt(user.Address);
            }
            catch
            {
                currentPhone = user.PhoneNumber ?? string.Empty;
                currentAddress = user.Address ?? string.Empty;
            }

            var newFullName = (EditInput.FullName ?? string.Empty).Trim();
            var newEmail = (EditInput.Email ?? string.Empty).Trim();
            var newPhone = (EditInput.Phone ?? string.Empty).Trim();
            var newAddress = (EditInput.Address ?? string.Empty).Trim();

            // ---- LOGIC KIỂM TRA BẢO MẬT EMAIL MỚI ----
            if (!string.Equals(user.Email ?? string.Empty, newEmail, StringComparison.OrdinalIgnoreCase))
            {
                // Kiểm tra xem đã hoàn thành bước xác thực email mới qua AJAX chưa
                var isNewEmailVerified = HttpContext.Session.GetString("NewEmailVerified") == "true";
                var pendingEmail = HttpContext.Session.GetString("PendingEmailChange");

                if (!isNewEmailVerified || !string.Equals(pendingEmail, newEmail, StringComparison.OrdinalIgnoreCase))
                {
                    TempData["ProfileError"] = "Security alert: Please complete the dual-email verification process before updating your email.";
                    return RedirectToPage();
                }

                // Nếu đã pass qua cả 2 lớp OTP -> Tiến hành cập nhật Email mới vào DB
                user.Email = newEmail;
                user.IsEmailConfirmed = true;

                // Dọn dẹp sạch Session sau khi dùng xong để tránh tái sử dụng token cũ
                HttpContext.Session.Remove("EmailChangeOldVerified");
                HttpContext.Session.Remove("PendingEmailChange");
                HttpContext.Session.Remove("NewEmailVerified");
            }

            // Cập nhật các thông tin cơ bản khác
            if (!string.Equals(user.FullName ?? string.Empty, newFullName, StringComparison.Ordinal))
            {
                user.FullName = newFullName;
            }

            if (!string.Equals(currentPhone, newPhone, StringComparison.Ordinal))
            {
                user.PhoneNumber = _encryptionService.Encrypt(newPhone);
            }

            if (!string.Equals(currentAddress, newAddress, StringComparison.Ordinal))
            {
                user.Address = _encryptionService.Encrypt(newAddress);
            }

            await _context.SaveChangesAsync();
            TempData["ProfileSuccess"] = "Profile and information updated successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostChangePasswordAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToPage("/Account/Login");
            }

            if (ChangePassword == null)
            {
                TempData["ProfileError"] = "Invalid password update request.";
                return RedirectToPage();
            }

            if (string.IsNullOrWhiteSpace(ChangePassword.CurrentPassword))
            {
                TempData["ProfileError"] = "Please enter your current password.";
                return RedirectToPage();
            }

            if (string.IsNullOrWhiteSpace(ChangePassword.NewPassword) || ChangePassword.NewPassword.Length < 8)
            {
                TempData["ProfileError"] = "New password must be at least 8 characters long.";
                return RedirectToPage();
            }

            if (!string.Equals(ChangePassword.NewPassword, ChangePassword.ConfirmPassword, StringComparison.Ordinal))
            {
                TempData["ProfileError"] = "New password and confirmation do not match.";
                return RedirectToPage();
            }

            int id = int.Parse(userIdStr);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return RedirectToPage("/Account/Login");
            }

            if (!_passwordHasher.Verify(ChangePassword.CurrentPassword, user.PasswordHash))
            {
                TempData["ProfileError"] = "The current password is incorrect.";
                return RedirectToPage();
            }

            user.PasswordHash = _passwordHasher.Hash(ChangePassword.NewPassword);
            await _context.SaveChangesAsync();

            TempData["ProfileSuccess"] = "Password updated successfully.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostSendPasswordOtpAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return new JsonResult(new { success = false, message = "Please sign in first." });
            }

            int id = int.Parse(userIdStr);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "User account not found." });
            }

            var now = DateTime.Now;
            if (user.CodeExpiry.HasValue && user.CodeExpiry.Value > now)
            {
                var remaining = user.CodeExpiry.Value - now;
                string waitText = remaining.TotalSeconds < 60
                    ? $"{Math.Ceiling(remaining.TotalSeconds)} seconds"
                    : $"{Math.Ceiling(remaining.TotalMinutes)} minutes";
                return new JsonResult(new { success = false, message = $"Please wait {waitText} before requesting another code." });
            }

            string otp = Random.Shared.Next(100000, 999999).ToString();
            user.EmailConfirmationCode = otp;
            user.CodeExpiry = now.AddMinutes(10);
            await _context.SaveChangesAsync();

            try
            {
                await _emailService.SendEmailAsync(user.Email, "Password reset verification", $"Your OTP is: {otp}");
                return new JsonResult(new { success = true, message = "OTP sent. Please check your email." });
            }
            catch
            {
                return new JsonResult(new { success = false, message = "Failed to send OTP. Please try again." });
            }
        }

        public async Task<IActionResult> OnPostSendOldEmailOtpAsync()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return new JsonResult(new { success = false, message = "Please sign in first." });
            }

            int id = int.Parse(userIdStr);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "User account not found." });
            }

            var now = DateTime.Now;
            if (user.CodeExpiry.HasValue && user.CodeExpiry.Value > now)
            {
                var remaining = user.CodeExpiry.Value - now;
                string waitText = remaining.TotalSeconds < 60
                    ? $"{Math.Ceiling(remaining.TotalSeconds)} seconds"
                    : $"{Math.Ceiling(remaining.TotalMinutes)} minutes";
                return new JsonResult(new { success = false, message = $"Please wait {waitText} before requesting another code." });
            }

            string otp = Random.Shared.Next(100000, 999999).ToString();
            user.EmailConfirmationCode = otp;
            user.CodeExpiry = now.AddMinutes(10);
            user.IsEmailConfirmed = false;
            await _context.SaveChangesAsync();

            try
            {
                await _emailService.SendEmailAsync(user.Email, "Verify your email", $"Your OTP is: {otp}");
                return new JsonResult(new { success = true, message = "OTP sent. Please check your email.", email = user.Email });
            }
            catch
            {
                return new JsonResult(new { success = false, message = "Failed to send OTP. Please try again." });
            }
        }

        public async Task<IActionResult> OnPostConfirmOldEmailOtpAsync([FromBody] ConfirmEmailChangeRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.OtpCode))
            {
                return new JsonResult(new { success = false, message = "Please provide the email and OTP." });
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return new JsonResult(new { success = false, message = "Please sign in first." });
            }

            int id = int.Parse(userIdStr);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "User account not found." });
            }

            var now = DateTime.Now;
            if (!string.Equals(user.EmailConfirmationCode, request.OtpCode, StringComparison.Ordinal))
            {
                return new JsonResult(new { success = false, message = "Invalid OTP code." });
            }

            if (!user.CodeExpiry.HasValue || user.CodeExpiry.Value <= now)
            {
                return new JsonResult(new { success = false, message = "OTP has expired. Please request a new code." });
            }

            HttpContext.Session.SetString("EmailChangeOldVerified", "true");
            user.EmailConfirmationCode = null;
            user.CodeExpiry = null;
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "Old email verified. Please verify your new email." });
        }

        public async Task<IActionResult> OnPostSendEmailOtpAsync([FromBody] EmailOtpRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
            {
                return new JsonResult(new { success = false, message = "Please provide a valid email." });
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return new JsonResult(new { success = false, message = "Please sign in first." });
            }

            if (HttpContext.Session.GetString("EmailChangeOldVerified") != "true")
            {
                return new JsonResult(new { success = false, message = "Please verify your current email first." });
            }

            int id = int.Parse(userIdStr);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "User account not found." });
            }

            var newEmail = request.Email.Trim();
            if (string.Equals(user.Email ?? string.Empty, newEmail, StringComparison.OrdinalIgnoreCase))
            {
                return new JsonResult(new { success = false, message = "This is already your current email." });
            }

            var now = DateTime.Now;
            if (user.CodeExpiry.HasValue && user.CodeExpiry.Value > now)
            {
                var remaining = user.CodeExpiry.Value - now;
                string waitText = remaining.TotalSeconds < 60
                    ? $"{Math.Ceiling(remaining.TotalSeconds)} seconds"
                    : $"{Math.Ceiling(remaining.TotalMinutes)} minutes";
                return new JsonResult(new { success = false, message = $"Please wait {waitText} before requesting another code." });
            }

            string otp = Random.Shared.Next(100000, 999999).ToString();
            user.EmailConfirmationCode = otp;
            user.CodeExpiry = now.AddMinutes(10);
            user.IsEmailConfirmed = false;
            HttpContext.Session.SetString("PendingEmailChange", newEmail);
            await _context.SaveChangesAsync();

            try
            {
                await _emailService.SendEmailAsync(newEmail, "Verify your new email", $"Your OTP is: {otp}");
                return new JsonResult(new { success = true, message = "OTP sent. Please check your email.", email = newEmail });
            }
            catch
            {
                return new JsonResult(new { success = false, message = "Failed to send OTP. Please try again." });
            }
        }

        public async Task<IActionResult> OnPostConfirmEmailChangeAsync([FromBody] ConfirmEmailChangeRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.OtpCode))
            {
                return new JsonResult(new { success = false, message = "Please provide the email and OTP." });
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return new JsonResult(new { success = false, message = "Please sign in first." });
            }

            // Kiểm tra an toàn bảo mật xem đã xác thực email cũ ở bước 1 chưa
            if (HttpContext.Session.GetString("EmailChangeOldVerified") != "true")
            {
                return new JsonResult(new { success = false, message = "Access denied. Please verify your current email first." });
            }

            var pendingEmail = HttpContext.Session.GetString("PendingEmailChange");
            if (string.IsNullOrWhiteSpace(pendingEmail) || !string.Equals(pendingEmail, request.Email.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return new JsonResult(new { success = false, message = "Session mismatch. Please request a new OTP code for the new email." });
            }

            int id = int.Parse(userIdStr);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
            {
                return new JsonResult(new { success = false, message = "User account not found." });
            }

            var now = DateTime.Now;
            if (!string.Equals(user.EmailConfirmationCode, request.OtpCode, StringComparison.Ordinal))
            {
                return new JsonResult(new { success = false, message = "Invalid OTP code for the new email." });
            }

            if (!user.CodeExpiry.HasValue || user.CodeExpiry.Value <= now)
            {
                return new JsonResult(new { success = false, message = "OTP has expired. Please request a new code." });
            }

            // CHỐT CHẶN: Đánh dấu xác thực vòng 2 thành công rực rỡ
            HttpContext.Session.SetString("NewEmailVerified", "true");

            // Xóa sạch code tạm để bảo mật
            user.EmailConfirmationCode = null;
            user.CodeExpiry = null;
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, message = "New email identity confirmed! Saving your profile changes..." });
        }

        public class EditProfileInput
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
            public string CurrentPassword { get; set; }
        }

        public class ChangePasswordInput
        {
            public string CurrentPassword { get; set; }
            public string NewPassword { get; set; }
            public string ConfirmPassword { get; set; }
        }

        public class EmailOtpRequest
        {
            public string Email { get; set; }
        }

        public class ConfirmEmailChangeRequest
        {
            public string Email { get; set; }
            public string OtpCode { get; set; }
        }
    }
}
