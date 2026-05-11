using System.ComponentModel.DataAnnotations;

namespace LaptopEcommerceAndMore.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailID { get; set; }

        public int OrderID { get; set; }

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Products Product { get; set; }
    }
}

