using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Interfaces;
using WebActionResults1923050471.Models;
using WebActionResults1923050471.ViewComponents;

namespace WebActionResults1923050471.Controllers
{
    public class CompareController(IDataService dataService, ICompareService compareService) : Controller
    {
        private readonly IDataService _dataService = dataService;
        private readonly ICompareService _compareService = compareService;
        private readonly CompareButtonViewModel _compareButtonViewModel; // Added access to CompareButtonViewModel

        public async Task<IActionResult> Index()
        {
            var ids = await _compareService.GetComparedProductIdsAsync(HttpContext);
            var products = new List<Product>();
            foreach (var id in ids)
            {
                var product = await _dataService.GetProductByIdAsync(id);
                if (product != null)
                {
                    products.Add(product);
                }
            }

            var viewModel = new CompareViewModel(products);
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(int productId)
        {
            var ids = await _compareService.GetComparedProductIdsAsync(HttpContext);
            if (ids.Contains(productId))
            {
                await _compareService.RemoveProductAsync(HttpContext, productId);
            }
            else
            {
                var added = await _compareService.AddProductAsync(HttpContext, productId);
                if (!added)
                {
                    TempData["CompareError"] = "You can compare up to 4 products.";
                }
            }

            if (Request.Headers.XRequestedWith == "XMLHttpRequest")
            {
                var updatedIds = await _compareService.GetComparedProductIdsAsync(HttpContext);
                var model = new CompareButtonViewModel(productId, updatedIds.Contains(productId), updatedIds.Count);
                return PartialView("~/Views/Shared/Components/CompareButton/Updated.cshtml", model);
            }

            var returnUrl = Request.Headers.Referer.ToString();
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index");
        }
    }

    public record CompareViewModel(IReadOnlyList<Product> Products);
}
