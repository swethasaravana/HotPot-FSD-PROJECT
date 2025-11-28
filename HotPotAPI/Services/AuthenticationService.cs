using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Repositories;
using System.Security.Cryptography;

namespace HotPotAPI.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<int, Customer> _customerRepository;
        private readonly IRepository<int, Admin> _adminRepository;
        private readonly IRepository<int, DeliveryPartner> _partnerRepository;
        private readonly IRepository<int, RestaurantManager> _managerRepository;

        private readonly ITokenService _tokenService;

        public AuthenticationService(IRepository<string, User> userRpository,
                                     IRepository<int, Customer> customerRepository,
                                     IRepository<int, Admin> adminrepository,
                                     IRepository<int, DeliveryPartner> partnerRepository,
                                     IRepository<int, RestaurantManager> managerRepository,
                                     ITokenService tokenService)
        {
            _userRepository = userRpository;
            _customerRepository = customerRepository;
            _adminRepository = adminrepository;
            _partnerRepository = partnerRepository;
            _tokenService = tokenService;
            _managerRepository= managerRepository;
        }

        
        public async Task<LoginResponse> Login(UserLoginRequest loginRequest)
        {
            var user = await _userRepository.GetById(loginRequest.Username);
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            HMACSHA512 hmac = new HMACSHA512(user.HashKey);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(loginRequest.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.Password[i])
                    throw new UnauthorizedAccessException("Invalid password");
            }

            string name = "";
            int id = 0;

            if (user.Role == "Customer")
            {
                var customer = (await _customerRepository.GetAll()).FirstOrDefault(c => c.Email == loginRequest.Username);
                if (customer == null)
                    throw new UnauthorizedAccessException("Customer not found");
                name = customer.Name;
                id = customer.Id;
            }
            else if (user.Role == "Admin")
            {
                // Similar logic if you have AdminRepository
                var admin = (await _adminRepository.GetAll()).FirstOrDefault(c => c.Email == loginRequest.Username);
                // or query from admin repo
                if (admin == null)
                    throw new UnauthorizedAccessException("Admin not found");
                name = admin.Name;
                id = admin.Id;
            }
            else if (user.Role == "DeliveryPartner")
            {
                var partner = (await _partnerRepository.GetAll()).FirstOrDefault(c => c.Email == loginRequest.Username);
                if (partner == null)
                    throw new UnauthorizedAccessException("Admin not found");
                name = partner.FullName;
                id = partner.DeliveryPartnerId;
            }
            else if (user.Role == "RestaurantManager")
            {
                var manager = (await _managerRepository.GetAll()).FirstOrDefault(c => c.Email == loginRequest.Username);
                if (manager == null)
                    throw new UnauthorizedAccessException("Restaurant Manager not found");
                name = manager.Username;
                id = manager.ManagerId;
            }
            else
            {
                throw new UnauthorizedAccessException("Invalid role");
            }


            var token = await _tokenService.GenerateToken(id, name, user.Role);
            return new LoginResponse { Id = id, Name = name, Role = user.Role, Token = token };
        }
    }
}
