using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Models;
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.Controllers
{
    public class AccountController(IDataService dataService) : Controller
    {
        private readonly IDataService _dataService = dataService;

        public async Task<IActionResult> Index()
        {
            var accounts = await _dataService.GetAllUsersAsync();
            return View(accounts);
        }

        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required");
                ViewData["ReturnUrl"] = returnUrl;
                return View();
            }

            if (await _dataService.ValidateLoginAsync(userName, password))
            {
                var account = await _dataService.GetUserByUserNameAsync(userName);
                if (account == null)
                {
                    ModelState.AddModelError("", "Invalid username or password");
                    ViewData["ReturnUrl"] = returnUrl;
                    return View();
                }
                HttpContext.Session.SetInt32("UserId", account.UserId);
                HttpContext.Session.SetString("UserName", account.Username);
                HttpContext.Session.SetString("FullName", account.FullName);
                if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return LocalRedirect(returnUrl);
                }
                return RedirectToAction("Index", "Product");
            }

            ModelState.AddModelError("", "Invalid username or password");
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Users account)
        {
            if (string.IsNullOrEmpty(account.Username) || string.IsNullOrEmpty(account.PasswordHash))
            {
                ModelState.AddModelError("", "Username and password are required");
                return View(account);
            }

            if (await _dataService.GetUserByUserNameAsync(account.Username) != null)
            {
                ModelState.AddModelError("", "Username already exists");
                return View(account);
            }

            await _dataService.AddUserAsync(account);
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var account = await _dataService.GetUserByIdAsync(userId.Value);
            if (account == null)
                return NotFound();

            return View(account);
        }

        public async Task<IActionResult> Account()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var account = await _dataService.GetUserByIdAsync(userId.Value);
            if (account == null)
                return NotFound();

            return View(account);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId != id)
                return RedirectToAction("Login");

            var account = await _dataService.GetUserByIdAsync(id);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Users account)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId != id)
                return RedirectToAction("Login");

            if (id != account.UserId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _dataService.UpdateUserAsync(account);
                HttpContext.Session.SetString("FullName", account.FullName);
                return RedirectToAction("Profile");
            }

            return View(account);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Product");
        }

        public async Task<IActionResult> Details(int id)
        {
            var account = await _dataService.GetUserByIdAsync(id);
            if (account == null)
                return NotFound();

            return View(account);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var account = await _dataService.GetUserByIdAsync(id);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dataService.DeleteUserAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
