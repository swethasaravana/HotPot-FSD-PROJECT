namespace HotPotAPI.Models.DTOs
{
    public class CustomerWithAddress
    {

        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public List<AddressDTO> Addresses { get; set; } = new();
    }

    public class AddressDTO
    {
        public string Label { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
    }
}