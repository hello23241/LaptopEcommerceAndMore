using Microsoft.AspNetCore.Mvc;
using LaptopEcommerceAndMore.Models;
using LaptopEcommerceAndMore.Interfaces;

namespace LaptopEcommerceAndMore.Controllers
{
    public class CategoryController(IDataService dataService) : Controller
    {
        private readonly IDataService _dataService = dataService;

        public async Task<IActionResult> Index()
        {
            var categories = await _dataService.GetAllCategoriesAsync();
            return View(categories);
        }

        public async Task<IActionResult> Details(int id)
        {
            var category = await _dataService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Categories category)
        {
            if (ModelState.IsValid)
            {
                await _dataService.AddCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _dataService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Categories category)
        {
            if (id != category.CategoryId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _dataService.UpdateCategoryAsync(category);
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var category = await _dataService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _dataService.DeleteCategoryAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

