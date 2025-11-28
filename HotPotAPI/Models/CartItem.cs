using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotPotAPI.Models
{
    public class CartItem
    {
        [Key]
        public int CartItemId { get; set; }

        [Required]
        public int CartId { get; set; }

        [ForeignKey("CartId")]
        public Cart Cart { get; set; }

        [Required]
        public int MenuItemId { get; set; }

        [ForeignKey("MenuItemId")]
        public MenuItem MenuItem { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; } = 1;

        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal PriceAtPurchase { get; set; }
    }
}
