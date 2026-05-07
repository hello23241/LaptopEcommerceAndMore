namespace WebActionResults1923050471.Models
{
    public class CartItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class ShoppingCart
    {
        public int AccountId { get; set; }
        public List<CartItem> Items { get; set; } = new List<CartItem>();
        public int TotalItems { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
