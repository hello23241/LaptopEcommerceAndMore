using WebActionResults1923050471.Models;

namespace WebActionResults1923050471.ViewModels
{
    public class ShoppingCart
    {
        public int UserID { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}