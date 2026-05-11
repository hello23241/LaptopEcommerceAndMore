using Microsoft.AspNetCore.Mvc;
using LaptopEcommerceAndMore.Interfaces;

namespace LaptopEcommerceAndMore.ViewComponents
{
    public class CompareButtonViewComponent(ICompareService compareService) : ViewComponent
    {
        private readonly ICompareService _compareService = compareService;

        public async Task<IViewComponentResult> InvokeAsync(int productId)
        {
            var ids = await _compareService.GetComparedProductIdsAsync(HttpContext);
            var model = new CompareButtonViewModel(productId, ids.Contains(productId), ids.Count);
            return View(model);
        }
    }

    public record CompareButtonViewModel(int ProductId, bool IsCompared, int Count);
}

