using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Models;
using WebActionResults1923050471.Services;

namespace WebActionResults1923050471.Controllers
{
    public class AccountController : Controller
    {
        private readonly IDataService _dataService;

        public AccountController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            var accounts = _dataService.GetAllAccounts();
            return View(accounts);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Username and password are required");
                return View();
            }

            if (_dataService.ValidateLogin(userName, password))
            {
                var account = _dataService.GetAccountByUserName(userName);
                HttpContext.Session.SetInt32("UserId", account.Id);
                HttpContext.Session.SetString("UserName", account.UserName);
                HttpContext.Session.SetString("FullName", account.FullName);
                return RedirectToAction("Index", "Product");
            }

            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Account account)
        {
            if (string.IsNullOrEmpty(account.UserName) || string.IsNullOrEmpty(account.Password))
            {
                ModelState.AddModelError("", "Username and password are required");
                return View(account);
            }

            if (_dataService.GetAccountByUserName(account.UserName) != null)
            {
                ModelState.AddModelError("", "Username already exists");
                return View(account);
            }

            _dataService.AddAccount(account);
            return RedirectToAction("Login");
        }

        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login");

            var account = _dataService.GetAccountById(userId.Value);
            if (account == null)
                return NotFound();

            return View(account);
        }

        public IActionResult Edit(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId != id)
                return RedirectToAction("Login");

            var account = _dataService.GetAccountById(id);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [HttpPost]
        public IActionResult Edit(int id, Account account)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || userId != id)
                return RedirectToAction("Login");

            if (id != account.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _dataService.UpdateAccount(account);
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

        public IActionResult Details(int id)
        {
            var account = _dataService.GetAccountById(id);
            if (account == null)
                return NotFound();

            return View(account);
        }

        public IActionResult Delete(int id)
        {
            var account = _dataService.GetAccountById(id);
            if (account == null)
                return NotFound();

            return View(account);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _dataService.DeleteAccount(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
