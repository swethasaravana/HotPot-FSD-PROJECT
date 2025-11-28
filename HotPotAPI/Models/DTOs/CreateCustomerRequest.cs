namespace HotPotAPI.Models.DTOs
{
    public class CreateCustomerRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public List<CreateCustomerAddress>? Addresses { get; set; }

    }
}
