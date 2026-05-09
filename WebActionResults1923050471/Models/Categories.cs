using System.ComponentModel.DataAnnotations;

namespace WebActionResults1923050471.Models
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }

        public string Description { get; set; }

        public string Icon { get; set; }

        public string Slug { get; set; }

        public int? ParentID { get; set; }

        public virtual Categories ParentCategory { get; set; }
        public virtual ICollection<Categories> Children { get; set; } = new List<Categories>();
        public virtual ICollection<Products> Products { get; set; } = new List<Products>();
    }
}
