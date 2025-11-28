using System.ComponentModel.DataAnnotations;

namespace HotPotAPI.Models.DTOs
{
    public class CreateCartItemRequest
    {
        [Required]
        public int MenuItemId { get; set; }

        [Required]
        public int Quantity { get; set; } = 1;
    }
}
