using System.ComponentModel.DataAnnotations;

namespace WebActionResults1923050471.Models
{
    public class Brands
    {
        [Key]
        public int BrandId { get; set; }

        [Required]
        public string BrandName { get; set; }

        public string BrandLogo { get; set; }
        public string ContactInfo { get; set; }
        public string Address { get; set; }
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
