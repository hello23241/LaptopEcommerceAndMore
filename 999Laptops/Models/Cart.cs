using System.ComponentModel.DataAnnotations;

namespace LaptopEcommerceAndMore.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        public int UserID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;

        public virtual Users User { get; set; }
        public virtual Products Product { get; set; }
    }
}

