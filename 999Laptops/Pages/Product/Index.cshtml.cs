using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaptopEcommerceAndMore.Pages.Product
{
    public class IndexModel : PageModel
    {
        private readonly IDataService _dataService;

        public IndexModel(IDataService dataService)
        {
            _dataService = dataService;
        }

        // Khai báo danh sách sản phẩm để phía Frontend (.cshtml) gọi qua Model.Products
        public List<Products> Products { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            // Lấy danh sách sản phẩm (Ví dụ lấy trang đầu tiên với số lượng lớn để quản trị)
            var allProducts = await _dataService.GetProductsPageAsync(1, 100);

            if (allProducts != null)
            {
                Products = allProducts;
            }

            return Page();
        }
    }
}