namespace HotPotAPI.Models.DTOs
{
    public class CreateMenuItemResponse
    {
        public int Id { get; set; }
        public string ItemName { get; set; }

        public string? ImagePath { get; set; }
        public string Description { get; set; }
        public TimeSpan? CookingTime { get; set; }
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
        public string Cuisine { get; set; }
        public string MealType { get; set; }
        public int RestaurantId { get; set; }
        public string RestaurantName { get; set; }

        public string tasteInfo { get; set; }
    }
}

