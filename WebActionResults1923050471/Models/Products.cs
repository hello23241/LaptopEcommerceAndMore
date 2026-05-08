using System.ComponentModel.DataAnnotations;

namespace WebActionResults1923050471.Models
{
    public class Products
    {
        [Key]
        public int ProductID { get; set; }

        [Required]
        public string ProductName { get; set; }

        public int BrandID { get; set; }

        public int CategoryID { get; set; }

        public decimal BasePrice { get; set; }

        public int StockQuantity { get; set; }

        public string ProductImage { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Brands Brand { get; set; }
        public virtual Categories Category { get; set; }
        public virtual ProductDetails ProductDetails { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
        public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
        public virtual ICollection<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
        public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
    }
}
