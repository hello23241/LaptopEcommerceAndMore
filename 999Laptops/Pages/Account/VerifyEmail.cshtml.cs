using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LaptopEcommerceAndMore.Data;
using LaptopEcommerceAndMore.Interfaces;

namespace LaptopEcommerceAndMore.Pages.Account
{
    public class VerifyEmailModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public VerifyEmailModel(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        [BindProperty]
        public string OtpCode { get; set; }

        public bool ShowResend { get; set; }
        public string StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            var mailFlag = TempData["MailStatus"]?.ToString();

            if (mailFlag == "Success")
            {
                StatusMessage = "Mã xác th?c dã du?c g?i thành công!";
            }
            else if (mailFlag == "Failed")
            {
                StatusMessage = "G?i mail th?t b?i. Vui lòng ki?m tra l?i email ho?c g?i l?i mã.";
            }

            ShowResend = (user != null && user.CodeExpiry < DateTime.Now) || (mailFlag == "Failed");
        }

        public async Task<IActionResult> OnPostResendAsync()
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);
            if (user != null)
            {
                string newOtp = Random.Shared.Next(100000, 999999).ToString();
                user.EmailConfirmationCode = newOtp;
                user.CodeExpiry = DateTime.Now.AddMinutes(10);
                await _context.SaveChangesAsync();

                try
                {
                    await _emailService.SendEmailAsync(user.Email, "Mã xác th?c m?i", $"Mã m?i là: {newOtp}");
                    TempData["MailStatus"] = "Success";
                }
                catch
                {
                    TempData["MailStatus"] = "Failed";
                }
            }
            return RedirectToPage(new { email = Email });
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (string.IsNullOrEmpty(OtpCode) || OtpCode.Length < 6)
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nh?p d? 6 ch? s?.");
                return Page();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Không tìm th?y thông tin ngu?i dùng.");
                return Page();
            }
            if (user.EmailConfirmationCode == OtpCode)
            {
                if (user.CodeExpiry > DateTime.Now)
                {
                    user.IsEmailConfirmed = true;
                    user.Role = "Customer";
                    user.EmailConfirmationCode = null;
                    user.CodeExpiry = null;

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Successfully verified your email. You can now log in.";
                    return RedirectToPage("Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "OTP has expired. Please log in again to receive a new code.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "The verification code is incorrect.");
            }

            return Page();
        }
    }
}
