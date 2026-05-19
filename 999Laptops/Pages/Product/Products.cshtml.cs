using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LaptopEcommerceAndMore.Interfaces;
using LaptopEcommerceAndMore.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaptopEcommerceAndMore.Pages.Product
{
    public class ProductsModel : PageModel
    {
        private readonly IDataService _dataService;
        private readonly ICurrencyService _currencyService; // Khai báo thêm dịch vụ tỷ giá

        public ProductsModel(IDataService dataService, ICurrencyService currencyService)
        {
            _dataService = dataService;
            _currencyService = currencyService;
        }

        public List<Products> Products { get; private set; } = new();
        public int PageNumber { get; private set; }
        public int TotalPages { get; private set; }
        public int? CategoryId { get; private set; }
        public int? BrandId { get; private set; }
        public string? CategoryName { get; private set; }
        public string? BrandName { get; private set; }
        public decimal UsdToVndRate { get; private set; } = 25400m; // Biến chứa tỷ giá thực tế

        // ĐA SỬA: Đổi tên tham số nhận diện từ 'page' thành 'p' để tránh từ khóa cấm
        public async Task<IActionResult> OnGetAsync(int p = 1, int? categoryId = null, int? brandId = null)
        {
            const int pageSize = 9;
            PageNumber = p < 1 ? 1 : p; // Gán lại giá trị dựa vào biến p
            CategoryId = categoryId;
            BrandId = brandId;
            if (CategoryId.HasValue)
            {
                CategoryName = (await _dataService.GetCategoryByIdAsync(CategoryId.Value))?.CategoryName;
            }

            if (BrandId.HasValue)
            {
                BrandName = (await _dataService.GetBrandByIdAsync(BrandId.Value))?.BrandName;
            }

            var totalCount = await _dataService.GetProductCountAsync(CategoryId, BrandId);
            TotalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            if (PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }

            UsdToVndRate = await _currencyService.GetUsdToVndRateAsync();
            Products = await _dataService.GetProductsPageAsync(PageNumber, pageSize, CategoryId, BrandId);

            return Page();
        }
    }
}