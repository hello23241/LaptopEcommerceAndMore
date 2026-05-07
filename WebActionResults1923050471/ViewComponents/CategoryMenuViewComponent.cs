using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Interfaces;

namespace WebActionResults1923050471.ViewComponents
{
    public class CategoryMenuViewComponent(IDataService dataService) : ViewComponent
    {
        private readonly IDataService _dataService = dataService;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _dataService.GetAllCategoriesAsync();
            return View(categories);
        }
    }
}
