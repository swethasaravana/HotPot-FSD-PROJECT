using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace HotPotAPI.Models
{
    public class OrderItem
    {
        [Key]
        public int OrderItemId { get; set; }

        // Foreign key to Order
        public int OrderId { get; set; }
        
        [JsonIgnore]
        public Order Order { get; set; }

        // Foreign key to MenuItem
        public int MenuItemId { get; set; }
        public MenuItem MenuItem { get; set; }

        public int Quantity { get; set; } 
        public decimal Price { get; set; } 
    }
}