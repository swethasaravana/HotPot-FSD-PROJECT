using HotPotAPI.Interfaces;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Models;
using System.Security.Cryptography;
using System.Text;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using HotPotAPI.Contexts;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace HotPotAPI.Services
{
    public class AdminService : IAdminService
    {
        private readonly HotPotDbContext _context;
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<int, Admin> _adminRepository;
        private readonly IRepository<int, Restaurant> _restaurantRepository;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            HotPotDbContext context,
            IRepository<string, User> userRepository,
            IRepository<int, Admin> adminRepository,
            IRepository<int, Restaurant> restaurantRepository,
            IRepository<int, Customer> customerRepository,
            ILogger<AdminService> logger)
        {
            _context = context;
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _restaurantRepository = restaurantRepository;
            _logger = logger;
        }

        public async Task<CreateAdminResponse> CreateAdmin(CreateAdminRequest request)
        {
            _logger.LogInformation("Creating admin with email: {Email}", request.Email);

            HMACSHA512 hmac = new HMACSHA512();
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            var user = MapAdminToUser(request, passwordHash, hmac.Key);

            var userResult = await _userRepository.Add(user);
            if (userResult == null)
            {
                _logger.LogError("Failed to create user for admin {Email}", request.Email);
                throw new Exception("Failed to create user");
            }

            var admin = MapAdmin(request);
            admin.User = user;

            var adminResult = await _adminRepository.Add(admin);
            if (adminResult == null)
            {
                _logger.LogError("Failed to create admin entry in admin repository for {Email}", request.Email);
                throw new Exception("Failed to create administrator");
            }

            _logger.LogInformation("Admin created successfully with ID: {AdminId}", adminResult.Id);
            return new CreateAdminResponse { Id = adminResult.Id };
        }

        private Admin MapAdmin(CreateAdminRequest request)
        {
            return new Admin
            {
                Name = request.Name,
                Email = request.Email
            };
        }

        private User MapAdminToUser(CreateAdminRequest request, byte[] passwordHash, byte[] key)
        {
            return new User
            {
                Username = request.Email,
                Password = passwordHash,
                HashKey = key,
                Role = "Admin"
            };
        }

        public async Task<Restaurant> AddRestaurant(CreateRestaurant dto)
        {
            _logger.LogInformation("Adding new restaurant: {Name}", dto.Name);

            if (string.IsNullOrWhiteSpace(dto.Name))
            {
                _logger.LogWarning("Restaurant name is missing.");
                throw new ArgumentException("Restaurant name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Address))
            {
                _logger.LogWarning("Restaurant address is missing.");
                throw new ArgumentException("Restaurant address is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Contact))
            {
                _logger.LogWarning("Restaurant contact is missing.");
                throw new ArgumentException("Restaurant contact number is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                _logger.LogWarning("Restaurant email is missing.");
                throw new ArgumentException("Restaurant email is required.");
            }

            if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                _logger.LogWarning("Invalid email format for restaurant: {Email}", dto.Email);
                throw new ArgumentException("Invalid email format.");
            }

            var restaurant = new Restaurant
            {
                RestaurantName = dto.Name,
                Location = dto.Address,
                ContactNumber = dto.Contact,
                Email = dto.Email,
                Restaurantlogo = dto.Restaurantlogo
            };

            var result = await _restaurantRepository.Add(restaurant);
            _logger.LogInformation("Restaurant added successfully with ID: {Id}", result.RestaurantId);
            return result;
        }

        public async Task<Restaurant> UpdateRestaurant(int id, CreateRestaurant dto)
        {
            _logger.LogInformation("Updating restaurant with ID: {Id}", id);

            var existing = await _restaurantRepository.GetById(id);
            if (existing == null)
            {
                _logger.LogWarning("Restaurant with ID {Id} not found.", id);
                throw new Exception("Restaurant not found");
            }

            existing.RestaurantName = dto.Name;
            existing.Location = dto.Address;
            existing.ContactNumber = dto.Contact;
            existing.Email = dto.Email;
            existing.Restaurantlogo = dto.Restaurantlogo;

            var updated = await _restaurantRepository.Update(existing.RestaurantId, existing);
            _logger.LogInformation("Restaurant with ID {Id} updated successfully.", id);
            return updated;
        }

        public async Task<bool> DeleteRestaurant(int id)
        {
            _logger.LogInformation("Attempting to delete restaurant with ID: {Id}", id);

            try
            {
                var restaurant = await _restaurantRepository.GetById(id);
                if (restaurant == null)
                {
                    _logger.LogWarning("Restaurant with ID {Id} not found for deletion.", id);
                    return false;
                }

                var deleted = await _restaurantRepository.Delete(id);
                _logger.LogInformation("Restaurant with ID {Id} deleted successfully.", id);
                return deleted != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting restaurant with ID: {Id}", id);
                return false;
            }
        }

        public async Task<List<Restaurant>> GetAllRestaurants()
        {
            _logger.LogInformation("Fetching all restaurants.");
            var restaurants = await _restaurantRepository.GetAll();
            return restaurants.ToList();
        }

        public async Task<Restaurant> GetRestaurantById(int id)
        {
            _logger.LogInformation("Fetching restaurant with ID: {Id}", id);
            var restaurant = await _restaurantRepository.GetById(id);

            if (restaurant == null)
            {
                _logger.LogWarning("Restaurant with ID {Id} not found.", id);
                throw new KeyNotFoundException($"Restaurant with ID {id} not found.");
            }

            return restaurant;
        }

        public async Task<int> GetTotalRestaurants()
        {
            _logger.LogInformation("Getting total restaurant count.");
            return await _context.Restaurants.CountAsync();
        }
    }
}
