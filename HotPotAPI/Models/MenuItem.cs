using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotPotAPI.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }
        
        [Required]
        public TimeSpan? CookingTime { get; set; }

        [Required, Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        [MaxLength(255)]
        public string? ImagePath { get; set; }

        [MaxLength(50)]
        public string AvailabilityTime { get; set; }

        public bool IsAvailable { get; set; }

        // Foreign Keys
        [Required]
        public int CuisineId { get; set; }
        [ForeignKey("CuisineId")]
        public Cuisine Cuisine { get; set; }

        [Required]
        public int MealTypeId { get; set; }
        [ForeignKey("MealTypeId")]
        public MealType MealType { get; set; }

        [Required]
        public int RestaurantId { get; set; }  
        [ForeignKey("RestaurantId")]
        public Restaurant Restaurant { get; set; }

        // Nutritional Info
        public int? Calories { get; set; }
        public float? Proteins { get; set; }
        public float? Fats { get; set; }
        public float? Carbohydrates { get; set; }

        // Additional Details
        [MaxLength(255)]
        public string TasteInfo { get; set; }
    }
}
