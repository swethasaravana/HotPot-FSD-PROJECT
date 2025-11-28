using HotPotAPI.Models.DTOs;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace HotPotAPI.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
 
        public User? User { get; set; } //navigation
        public Cart? Cart { get; set; }
        public ICollection<CustomerAddress>? Addresses { get; set; }
        [JsonIgnore]
        public ICollection<Order> Orders { get; set; }


    }
}
