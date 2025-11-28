using System.ComponentModel.DataAnnotations;

namespace HotPotAPI.Models
{
    public class PaymentMethod
    {
        [Key]
        public int PaymentMethodId { get; set; }

        [Required]
        [MaxLength(40)]
        public string MethodName { get; set; }
    }
}
