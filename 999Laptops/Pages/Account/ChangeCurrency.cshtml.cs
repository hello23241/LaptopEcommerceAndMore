using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LaptopEcommerceAndMore.Pages.Account
{
    public class ChangeCurrencyModel : PageModel
    {
        public IActionResult OnGet(string currency, string returnUrl)
        {
            if (currency == "VND" || currency == "USD")
            {
                // Lưu lựa chọn vào Session toàn hệ thống
                HttpContext.Session.SetString("Currency", currency);
            }

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToPage("/Index");
        }
    }
}