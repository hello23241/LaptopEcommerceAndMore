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
            var suppliers = await _dataService.GetAllSuppliersAsync();
            return View(suppliers);
        }

        public async Task<IActionResult> Details(int id)
        {
            var supplier = await _dataService.GetSupplierByIdAsync(id);
            if (supplier == null)
                return NotFound();
            return View(supplier);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                await _dataService.AddSupplierAsync(supplier);
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var supplier = await _dataService.GetSupplierByIdAsync(id);
            if (supplier == null)
                return NotFound();
            return View(supplier);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Supplier supplier)
        {
            if (id != supplier.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _dataService.UpdateSupplierAsync(supplier);
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var supplier = await _dataService.GetSupplierByIdAsync(id);
            if (supplier == null)
                return NotFound();
            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dataService.DeleteSupplierAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
