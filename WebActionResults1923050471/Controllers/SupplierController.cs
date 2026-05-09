using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Models;
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.Controllers
{
    public class SupplierController(IDataService dataService) : Controller
    {
        private readonly IDataService _dataService = dataService;

        public async Task<IActionResult> Index()
        {
            var brand = await _dataService.GetAllBrandsAsync();
            return View(brand);
        }

        public async Task<IActionResult> Details(int id)
        {
            var brand = await _dataService.GetBrandByIdAsync(id);
            if (brand == null)
                return NotFound();
            return View(brand);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Brands brand)
        {
            if (ModelState.IsValid)
            {
                await _dataService.AddBrandAsync(brand);
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _dataService.GetBrandByIdAsync(id);
            if (supplier == null)
                return NotFound();
            return View(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Brands brand)
        {
            if (id != brand.BrandId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _dataService.UpdateBrandAsync(brand);
                return RedirectToAction(nameof(Index));
            }
            return View(brand);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var brand = await _dataService.GetBrandByIdAsync(id);
            if (brand == null)
                return NotFound();
            return View(brand);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dataService.DeleteBrandAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
