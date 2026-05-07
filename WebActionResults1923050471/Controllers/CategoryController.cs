using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Models;
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.Controllers
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
        public async Task<IActionResult> Create(Category category)
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
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.Id)
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
