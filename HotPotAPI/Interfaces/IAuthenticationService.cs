using HotPotAPI.Models.DTOs;

namespace HotPotAPI.Interfaces
{
    public interface IAuthenticationService
    {
        Task<LoginResponse> Login(UserLoginRequest loginRequest);

    }
}
