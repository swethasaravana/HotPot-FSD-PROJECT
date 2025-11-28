using System.ComponentModel.DataAnnotations;

namespace HotPotAPI.Models
{
    public class PaymentStatus
    {
        [Key]
        public int PaymentStatusId { get; set; }

        [Required]
        [MaxLength(40)]
        public string StatusName { get; set; }
    }
}
