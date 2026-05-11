using System.ComponentModel.DataAnnotations;

namespace LaptopEcommerceAndMore.Models
{
    public class Users
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 5)]
        [RegularExpression("^[a-z0-9]+$")]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string Role { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string? EmailConfirmationCode { get; set; }
        public DateTime? CodeExpiry { get; set; }
        public string PhoneNumber { get; set; }

        public string Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Orders> Orders { get; set; } = new List<Orders>();
        public virtual ICollection<Cart> CartItems { get; set; } = new List<Cart>();
        public virtual ICollection<Wishlist> WishlistItems { get; set; } = new List<Wishlist>();
        public virtual ICollection<Reviews> Reviews { get; set; } = new List<Reviews>();
    }
}

