using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotPotAPI.Models
{
    public class User
    {
        [Key]

        [MaxLength(100)]
        public string Username { get; set; } = string.Empty;
        public byte[] Password { get; set; }
        public byte[] HashKey { get; set; }
        public string Role { get; set; }

        public Customer? Customer { get; set; } // navigation
        public Admin? Admin { get; set; }
        public RestaurantManager? RestaurantManager { get; set; }
        public DeliveryPartner? DeliveryPartner { get; set; }

    }
}
