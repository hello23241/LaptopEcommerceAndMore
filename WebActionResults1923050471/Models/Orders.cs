using System.ComponentModel.DataAnnotations;

namespace WebActionResults1923050471.Models
{
    public class Orders
    {
        [Key]
        public int OrderID { get; set; }

        public int UserID { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public decimal TotalAmount { get; set; }

        public string ShippingAddress { get; set; }

        public string PaymentMethod { get; set; }

        public string Status { get; set; }

        public virtual Users User { get; set; }
        public virtual ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
    }
}
