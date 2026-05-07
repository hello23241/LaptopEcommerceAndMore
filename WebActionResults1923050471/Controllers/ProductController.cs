using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Models;
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.Controllers
{
    public class ProductController : Controller
    {
        private readonly IDataService _dataService;

        public ProductController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            var products = _dataService.GetAllProducts();
            return View(products);
        }

        public IActionResult Details(int id)
        {
            var product = _dataService.GetProductById(id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = _dataService.GetAllCategories();
            ViewBag.Suppliers = _dataService.GetAllSuppliers();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _dataService.AddProduct(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _dataService.GetAllCategories();
            ViewBag.Suppliers = _dataService.GetAllSuppliers();
            return View(product);
        }

        public IActionResult Edit(int id)
        {
            var product = _dataService.GetProductById(id);
            if (product == null)
                return NotFound();
            ViewBag.Categories = _dataService.GetAllCategories();
            ViewBag.Suppliers = _dataService.GetAllSuppliers();
            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(int id, Product product)
        {
            if (id != product.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _dataService.UpdateProduct(product);
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = _dataService.GetAllCategories();
            ViewBag.Suppliers = _dataService.GetAllSuppliers();
            return View(product);
        }

        public IActionResult Delete(int id)
        {
            var product = _dataService.GetProductById(id);
            if (product == null)
                return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _dataService.DeleteProduct(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
