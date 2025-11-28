namespace HotPotAPI.Models.DTOs
{
    public class DeliveryPartnerUpdate
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string VehicleNumber { get; set; }
        public bool IsAvailable { get; set; }
    }
}