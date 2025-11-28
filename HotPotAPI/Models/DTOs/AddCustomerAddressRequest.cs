namespace HotPotAPI.Models.DTOs
{
    public class AddCustomerAddressRequest
    {
        public string Label { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
    }
}