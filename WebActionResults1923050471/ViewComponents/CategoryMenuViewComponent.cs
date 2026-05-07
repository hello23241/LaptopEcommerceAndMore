using Microsoft.AspNetCore.Mvc;
using WebActionResults1923050471.Services;

namespace WebActionResults1923050471.ViewComponents
{
    public class CategoryMenuViewComponent : ViewComponent
    {
        private readonly IDataService _dataService;

        public CategoryMenuViewComponent(IDataService dataService)
        {
            _dataService = dataService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = _dataService.GetAllCategories();
            return View(categories);
        }
    }
}
