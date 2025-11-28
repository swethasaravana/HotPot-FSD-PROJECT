using System.ComponentModel.DataAnnotations;

namespace HotPotAPI.Models
{
    public class Cuisine
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }
    }
}
