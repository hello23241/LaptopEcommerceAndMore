using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Models;
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.Controllers
{
    public class SupplierController : Controller
    {
        private readonly IDataService _dataService;

        public SupplierController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult Index()
        {
            var suppliers = _dataService.GetAllSuppliers();
            return View(suppliers);
        }

        public IActionResult Details(int id)
        {
            var supplier = _dataService.GetSupplierById(id);
            if (supplier == null)
                return NotFound();
            return View(supplier);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                _dataService.AddSupplier(supplier);
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        public IActionResult Edit(int id)
        {
            var supplier = _dataService.GetSupplierById(id);
            if (supplier == null)
                return NotFound();
            return View(supplier);
        }

        [HttpPost]
        public IActionResult Edit(int id, Supplier supplier)
        {
            if (id != supplier.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                _dataService.UpdateSupplier(supplier);
                return RedirectToAction(nameof(Index));
            }
            return View(supplier);
        }

        public IActionResult Delete(int id)
        {
            var supplier = _dataService.GetSupplierById(id);
            if (supplier == null)
                return NotFound();
            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            _dataService.DeleteSupplier(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
