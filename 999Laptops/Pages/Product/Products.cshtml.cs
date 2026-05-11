using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.Models;

namespace LaptopEcommerceAndMore.Pages.Product
{
    public class ProductsModel(IDataService dataService) : PageModel
    {
        private readonly IDataService _dataService = dataService;
        public List<Products> Products { get; private set; } = new();
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }

        public async Task<IActionResult> OnGetAsync(int page = 1)
        {
            const int pageSize = 9;
            PageNumber = page < 1 ? 1 : page;
            var totalCount = await _dataService.GetProductCountAsync();
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            if (PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }

            Products = await _dataService.GetProductsPageAsync(PageNumber, pageSize);
            return Page();
        }
    }
}

