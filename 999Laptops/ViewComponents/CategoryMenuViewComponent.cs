using Microsoft.AspNetCore.Mvc;
using LaptopEcommerceAndMore.Interfaces;

namespace LaptopEcommerceAndMore.ViewComponents
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

