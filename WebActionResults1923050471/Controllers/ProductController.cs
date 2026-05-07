using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Models; // Ensure models namespace is used
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.Controllers
{
    public class ProductController(IDataService dataService) : Controller
    {
        private readonly IDataService _dataService = dataService;

        private static IReadOnlyList<ProductDetailTab> GetDetailTabs() =>
            new List<ProductDetailTab>
            {
                new("Description", "Designed for everyday performance with a clean, modern build and smooth multitasking."),
                new("Details", "Display: 15.6\" FHD | Memory: 16GB | Storage: 512GB SSD | Battery: Up to 10 hours"),
                new("Reviews", "Reviews are coming soon. Check back after launch.")
            };

        public async Task<IActionResult> Index()
        {
            var products = await _dataService.GetAllProductsAsync();
            return View(products);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _dataService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            var viewModel = new ProductDetailsViewModel(product, GetDetailTabs());
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _dataService.GetAllCategoriesAsync();
            ViewBag.Suppliers = await _dataService.GetAllSuppliersAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                await _dataService.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await _dataService.GetAllCategoriesAsync();
            ViewBag.Suppliers = await _dataService.GetAllSuppliersAsync();
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _dataService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            ViewBag.Categories = await _dataService.GetAllCategoriesAsync();
            ViewBag.Suppliers = await _dataService.GetAllSuppliersAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _dataService.UpdateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await _dataService.GetAllCategoriesAsync();
            ViewBag.Suppliers = await _dataService.GetAllSuppliersAsync();
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var product = await _dataService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dataService.DeleteProductAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
