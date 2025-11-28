using System.ComponentModel.DataAnnotations;

namespace HotPotAPI.Models
{
    public class OrderStatus
    {
        [Key]
        public int OrderStatusId { get; set; }

        [Required]
        [MaxLength(40)]
        public string StatusName { get; set; }
    }
}
