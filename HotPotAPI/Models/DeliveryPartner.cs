using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace HotPotAPI.Models
{
    public class DeliveryPartner
    {
        [Key]
        public int DeliveryPartnerId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string VehicleNumber { get; set; } 
        public bool IsAvailable { get; set; } = true;

        [ForeignKey("User")]
        public string Username { get; set; }  // Foreign Key

        [JsonIgnore]
        public User? User { get; set; }       // Navigation Property
    }
}
