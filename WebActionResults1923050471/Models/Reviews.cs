using System.ComponentModel.DataAnnotations;

namespace WebActionResults1923050471.Models
{
    public class Reviews
    {
        [Key]
        public int ReviewID { get; set; }

        public int UserID { get; set; }

        public int ProductID { get; set; }

        public int Rating { get; set; }

        public string Comment { get; set; }

        public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

        public bool IsPurchased { get; set; }

        public virtual Users User { get; set; }
        public virtual Products Product { get; set; }
    }
}
