using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotPotAPI.Models
{
    public class CustomerAddress
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required, MaxLength(255)]
        public string Label { get; set; } // e.g., Home, Office

        [Required, MaxLength(255)]
        public string Street { get; set; }

        [Required, MaxLength(100)]
        public string City { get; set; }

        [Required, MaxLength(10)]
        public string Pincode { get; set; }

        // Navigation Properties
        [ForeignKey("CustomerId")]
        public Customer Customer { get; set; }
    }
}
