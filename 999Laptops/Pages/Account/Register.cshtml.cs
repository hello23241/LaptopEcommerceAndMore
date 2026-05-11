using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore; // Quan tr?ng d? dùng AnyAsync
using System.ComponentModel.DataAnnotations;
using LaptopEcommerceAndMore.Data;
using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.Models;

namespace LaptopEcommerceAndMore.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEncryptionService _encryptionService;
        private readonly IEmailService _emailService;

        // CH? GI? L?I M?T CONSTRUCTOR DUY NH?T (Có d? 4 service)
        public RegisterModel(
            ApplicationDbContext context,
            IPasswordHasher passwordHasher,
            IEncryptionService encryptionService,
            IEmailService emailService)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _encryptionService = encryptionService;
            _emailService = emailService;
        }

        [BindProperty]
        public RegisterData Input { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            // S? d?ng AnyAsync d? không làm treo UI khi check DB
            var existingUser = await _context.Users.AnyAsync(u => u.Username == Input.Username || u.Email == Input.Email);
            if (existingUser)
            {
                ModelState.AddModelError(string.Empty, "Username or Email is already registered.");
                return Page();
            }

            // T?o mã OTP 6 s?
            string otpCode = Random.Shared.Next(100000, 999999).ToString();

            var user = new Users
            {
                Username = Input.Username,
                FullName = Input.FullName,
                Email = Input.Email,
                PhoneNumber = _encryptionService.Encrypt(Input.Phone ?? ""),
                Address = _encryptionService.Encrypt(Input.Address ?? ""),
                PasswordHash = _passwordHasher.Hash(Input.Password),
                Role = "Locked", // Tài kho?n b? khóa t?m th?i
                EmailConfirmationCode = otpCode,
                IsEmailConfirmed = false,
                CodeExpiry = DateTime.Now.AddMinutes(10)
            };

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // G?i mail xác th?c
                try
                {
                    string subject = "Verify your registration to 999 Laptops";
                string body = $@"
                    <div style='font-family: Arial, sans-serif; border: 1px solid #eee; padding: 20px; border-radius: 10px;'>
                        <h2 style='color: #D10024;'>Welcome to 999 Laptops!</h2>
                        <p>Hi <b>{user.FullName}</b>,</p>
                        <p>Your verification code is:</p>
                        <h1 style='background: #f8f9fa; display: inline-block; padding: 10px 20px; border: 2px dashed #D10024; color: #D10024;'>{otpCode}</h1>
                        <p>This code is valid for <b>10 minutes</b>.</p>
                        <p>If you didn't request this, you can safely ignore this email.</p>
                    </div>";

                await _emailService.SendEmailAsync(user.Email, subject, body);
                }
                catch
                {
                    // Flag g?i th?t b?i
                    TempData["MailStatus"] = "Failed";
                }
                // Chuy?n hu?ng sang trang Verify
                return RedirectToPage("VerifyEmail", new { email = user.Email });
            }
            catch (Exception ex)
            {
                // B?n có th? log ex.Message ra console d? debug
                ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
                return Page();
            }
        }

        public class RegisterData
        {
            [Required(ErrorMessage = "Username is required")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Full name is required")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Email is required")]
            [EmailAddress(ErrorMessage = "Invalid email address")]
            public string Email { get; set; }

            public string? Phone { get; set; }

            public DateTime? Birthday { get; set; }

            [Required(ErrorMessage = "Password is required")]
            [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Address is required")]
            public string Address { get; set; }
        }
    }
}
