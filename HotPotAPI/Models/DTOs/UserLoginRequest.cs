using System.ComponentModel.DataAnnotations;

namespace HotPotAPI.Models.DTOs
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
