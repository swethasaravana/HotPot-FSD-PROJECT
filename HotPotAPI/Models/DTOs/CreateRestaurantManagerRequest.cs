namespace HotPotAPI.Models.DTOs
{
    public class CreateRestaurantManagerRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int? RestaurantId { get; set; }
    }
}
