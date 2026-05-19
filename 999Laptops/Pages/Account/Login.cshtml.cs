using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using LaptopEcommerceAndMore.Data;
using LaptopEcommerceAndMore.Interfaces;

namespace LaptopEcommerceAndMore.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public LoginModel(IPasswordHasher passwordHasher, ApplicationDbContext context, IEmailService emailService)
        {
            _passwordHasher = passwordHasher;
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> OnGetLogout(string? returnUrl = null)
        {
            // 1. Xóa Session
            HttpContext.Session.Clear();

            // 2. Xóa Cookie xác th?c c?a h? th?ng
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (returnUrl != null && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToPage("/Index");
        }

        [BindProperty]
        public LoginData Input { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            if (!ModelState.IsValid) return Page();

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == Input.Username);

            if (user == null || !_passwordHasher.Verify(Input.Password, user.PasswordHash))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return Page();
            }

            if (user.Role == "Locked" || !user.IsEmailConfirmed)
            {
                string newOtp = Random.Shared.Next(100000, 999999).ToString();
                user.EmailConfirmationCode = newOtp;
                user.CodeExpiry = DateTime.Now.AddMinutes(10);
                await _context.SaveChangesAsync();

                await _emailService.SendEmailAsync(user.Email, "Verify your account", $"Your OTP is: {newOtp}");

                return RedirectToPage("VerifyEmail", new { email = user.Email });
            }


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role ?? "Customer"),
                new Claim("FullName", user.FullName ?? "")
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);


            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            HttpContext.Session.SetString("UserId", user.UserId.ToString());
            HttpContext.Session.SetString("UserName", user.Username);
            HttpContext.Session.SetString("UserRole", user.Role ?? "Customer");

            if (returnUrl != null && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return user.Role == "Admin" ? RedirectToPage("/Admin/Index") : RedirectToPage("/Index");
        }
    }

    public class LoginData
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
