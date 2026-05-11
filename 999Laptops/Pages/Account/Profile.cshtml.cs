using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using LaptopEcommerceAndMore.Data;
using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.Models;

namespace LaptopEcommerceAndMore.Pages.Account
{
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IEncryptionService _encryptionService;

        public ProfileModel(ApplicationDbContext context, IEncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        public Users CurrentUser { get; set; }
        public string DecryptedPhone { get; set; }
        public string DecryptedAddress { get; set; }
        public string MaskedPhone { get; set; }
        public string MaskedAddress { get; set; }
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

            return Page();
        }
    }
}
