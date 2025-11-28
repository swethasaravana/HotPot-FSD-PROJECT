namespace HotPotAPI.Models.DTOs
{
    public class CreateResturantManagerResponse
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? RestaurantName { get; set; }
    }
}
