using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Models; // Ensure models namespace is used
using WebActionResults1923050471.ViewModels;
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.Controllers
{
    public class ProductController(IDataService dataService) : Controller
    {
        private readonly IDataService _dataService = dataService;

        private static IReadOnlyList<ProductDetailTab> GetDetailTabs(Products product)
        {
            var tabs = new List<ProductDetailTab>
            {
                new("Description", product.ProductName),
                new("Reviews", "Reviews are coming soon. Check back after launch.")
            };

            if (product.ProductDetails != null)
            {
                var details = $"CPU: {product.ProductDetails.CPU}\nRAM: {product.ProductDetails.RAM}\nGPU: {product.ProductDetails.GPU}\nHardDrive: {product.ProductDetails.Storage}\nScreen: {product.ProductDetails.Display}\nWeight: {product.ProductDetails.Weight}\nExtra: {product.ProductDetails.OS}";
                tabs.Insert(1, new ProductDetailTab("Details", details));
            }

            return tabs;
        }

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
            var viewModel = new ProductDetailsViewModel(product, GetDetailTabs(product));
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _dataService.GetAllCategoriesAsync();
            ViewBag.Brands = await _dataService.GetAllBrandsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Products product)
        {
            if (ModelState.IsValid)
            {
                await _dataService.AddProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await _dataService.GetAllCategoriesAsync();
            ViewBag.Suppliers = await _dataService.GetAllBrandsAsync();
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _dataService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();
            ViewBag.Categories = await _dataService.GetAllCategoriesAsync();
            ViewBag.Suppliers = await _dataService.GetAllBrandsAsync();
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Products product)
        {
            if (id != product.ProductID)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _dataService.UpdateProductAsync(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = await _dataService.GetAllCategoriesAsync();
            ViewBag.Suppliers = await _dataService.GetAllBrandsAsync();
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
