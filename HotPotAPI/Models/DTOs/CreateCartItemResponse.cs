using System.ComponentModel.DataAnnotations;

namespace HotPotAPI.Models.DTOs
{
    public class CreateCartItemResponse
    {
        [Required]
        public int CartItemId { get; set; }
        public int MenuItemId { get; set; }
        public string MenuItemName { get; set; }

        [Required]
        public int Quantity { get; set; }
        public decimal PriceAtPurchase { get; set; }
        public decimal TotalPrice => Quantity * PriceAtPurchase;
    }
}
