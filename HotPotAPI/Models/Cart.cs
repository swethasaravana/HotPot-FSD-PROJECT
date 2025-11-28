using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HotPotAPI.Models
{
    public class Cart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Customer Customer { get; set; } // Navigation Properties
        public ICollection<CartItem> CartItems { get; set; }
    }
}
