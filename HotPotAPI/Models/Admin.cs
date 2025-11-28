using System.ComponentModel.DataAnnotations;

namespace HotPotAPI.Models
{
    public class Admin
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public User? User { get; set; }

    }
}
