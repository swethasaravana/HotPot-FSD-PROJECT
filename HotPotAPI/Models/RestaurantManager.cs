using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HotPotAPI.Models
{
    public class RestaurantManager
    {
        [Key]
        public int ManagerId { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; } 

        [Required]
        public string Email { get; set; }
        public string PhoneNumber {  get; set; }

        
        [ForeignKey("Restaurant")]
        public int RestaurantId { get; set; } // Foreign key

        // Navigation property
        public Restaurant Restaurant { get; set; }
        public User? User { get; set; }

    }
}
