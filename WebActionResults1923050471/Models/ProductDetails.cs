using System.ComponentModel.DataAnnotations;

namespace WebActionResults1923050471.Models
{
    public class ProductDetails
    {
        [Key]
        public int ProductID { get; set; }
        public string CPU { get; set; }

        public string RAM { get; set; }

        public string GPU { get; set; }

        public string Display { get; set; }

        public string Storage { get; set; }

        public string Battery { get; set; }

        public string Weight { get; set; }

        public string OS { get; set; }
        public string Extra { get; set; }
        public virtual Products Product { get; set; }
    }
}
