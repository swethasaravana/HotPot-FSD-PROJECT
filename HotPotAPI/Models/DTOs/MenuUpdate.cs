namespace HotPotAPI.Models.DTOs
{
    public class MenuItemUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public TimeSpan? CookingTime { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public string AvailabilityTime { get; set; }
        public bool IsAvailable { get; set; }
        public int CuisineId { get; set; }
        public int MealTypeId { get; set; }
        public int? Calories { get; set; }
        public float? Proteins { get; set; }
        public float? Fats { get; set; }
        public float? Carbohydrates { get; set; }
        public string TasteInfo { get; set; }
    }
}
