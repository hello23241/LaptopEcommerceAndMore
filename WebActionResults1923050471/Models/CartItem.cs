using System.ComponentModel.DataAnnotations;

namespace WebActionResults1923050471.Models
{
    public class CartItem
    {
        [Key]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Subtotal { get; set; }

        // Navigation property
        public virtual Products Product { get; set; }
    }
}