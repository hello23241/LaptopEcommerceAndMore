using LaptopEcommerceAndMore.Models;

namespace LaptopEcommerceAndMore.ViewModels
{
    public class ShoppingCart
    {
        public int UserID { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
