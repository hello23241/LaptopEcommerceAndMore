using LaptopEcommerceAndMore.Models;
namespace LaptopEcommerceAndMore.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Products Product { get; set; }
        public IReadOnlyList<ProductDetailTab> Tabs { get; set; }

        public ProductDetailsViewModel(Products product, IReadOnlyList<ProductDetailTab> tabs)
        {
            Product = product;
            Tabs = tabs;
        }
    }
}
