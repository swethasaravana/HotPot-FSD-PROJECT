using System;
using System.Globalization;
using System.Threading.Tasks;
using HotPotAPI.Interfaces;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Models;
using System.Security.Cryptography;
using System.Text;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using HotPotAPI.Contexts;

namespace HotPotAPI.Services
{
    public class RestaurantManagerService : IRestaurantManagerService
    {
        private readonly HotPotDbContext _context;
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<int, RestaurantManager> _managerRepository;
        private readonly IRepository<int, Restaurant> _restaurantRepository;
        private readonly IRepository<int, MenuItem> _menuItemRepository;

        public RestaurantManagerService(
            HotPotDbContext context,
            IRepository<string, User> userRepository,
            IRepository<int, RestaurantManager> managerRepository,
            IRepository<int, Restaurant> restaurantRepository, IRepository<int, MenuItem> menuItemRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _managerRepository = managerRepository;
            _restaurantRepository = restaurantRepository;
            _menuItemRepository = menuItemRepository;
        }

        public async Task<CreateResturantManagerResponse> AddRestaurantManager(CreateRestaurantManagerRequest request)
        {
            HMACSHA512 hmac = new HMACSHA512();
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            var user = MapManagerToUser(request, passwordHash, hmac.Key);

            var userResult = await _userRepository.Add(user);
            if (userResult == null)
                throw new Exception("Failed to create user");

            var manager = MapManager(request);
            manager.User = user; // set navigation property

            var managerResult = await _managerRepository.Add(manager);
            if (managerResult == null)
                throw new Exception("Failed to create manager");

            var restaurant = await _restaurantRepository.GetById(managerResult.RestaurantId);

            return new CreateResturantManagerResponse
            {
                Id = managerResult.ManagerId,
                FullName = managerResult.Username,
                Email = managerResult.Email,
                Phone = managerResult.PhoneNumber,
                RestaurantName = restaurant?.RestaurantName
            };
        }

        private RestaurantManager MapManager(CreateRestaurantManagerRequest request)
        {
            return new RestaurantManager
            {
                Username = request.FullName,
                Email = request.Email,
                Password = Convert.ToBase64String(Encoding.UTF8.GetBytes(request.Password)),
                PhoneNumber = request.PhoneNumber,
                RestaurantId = request.RestaurantId ?? 0
            };
        }

        private User MapManagerToUser(CreateRestaurantManagerRequest request, byte[] passwordHash, byte[] key)
        {
            return new User
            {
                Username = request.Email,
                Password = passwordHash,
                HashKey = key,
                Role = "RestaurantManager"
            };
        }

        public async Task<CreateResturantManagerResponse> UpdateManagerById(int managerId, RestaurantManagerUpdate request)
        {
            var existingManager = await _managerRepository.GetById(managerId);
            if (existingManager == null)
                throw new KeyNotFoundException("Manager not found");

            // Update only allowed fields
            existingManager.Username = request.Username;
            existingManager.Password = request.Password;
            existingManager.Email = request.Email;
            existingManager.PhoneNumber = request.PhoneNumber;

            // No updating RestaurantId

            var updatedManager = await _managerRepository.Update(managerId, existingManager);
            if (updatedManager == null)
                throw new Exception("Failed to update manager");

            var restaurant = await _restaurantRepository.GetById(updatedManager.RestaurantId);

            return new CreateResturantManagerResponse
            {
                Id = updatedManager.ManagerId,
                FullName = updatedManager.Username,
                Email = updatedManager.Email,
                Phone = updatedManager.PhoneNumber,
                RestaurantName = restaurant?.RestaurantName
            };
        }

        public async Task<bool> DeleteManagerById(int managerId)
        {
            var manager = await _context.RestaurantManagers.FindAsync(managerId);
            if (manager == null) return false;

            // Get the associated User
            var user = await _context.Users.FindAsync(manager.User);
            if (user != null)
            {
                _context.Users.Remove(user); // Delete user
            }

            _context.RestaurantManagers.Remove(manager); // Delete manager
            await _context.SaveChangesAsync(); // Save changes

            return true;
        }

        public async Task<CreateResturantManagerResponse> GetManagerById(int managerId)
        {
            var manager = await _managerRepository.GetById(managerId);

            if (manager == null)
                throw new Exception("Manager not found");

            var restaurant = await _restaurantRepository.GetById(manager.RestaurantId);

            return new CreateResturantManagerResponse
            {
                Id = manager.ManagerId,
                FullName = manager.Username,
                Email = manager.Email,
                Phone = manager.PhoneNumber,
                RestaurantName = restaurant?.RestaurantName
            };
        }


        public async Task<CreateMenuItemResponse> AddMenuItem(int managerId, CreateMenuItemRequest menuItemDto)
        {
            // Get the manager to find their restaurant
            var manager = await _managerRepository.GetById(managerId);
            if (manager == null)
                throw new Exception("Manager not found");

            // Map values from request and set restaurant from manager
            var newItem = new MenuItem
            {
                Name = menuItemDto.Name,
                Description = menuItemDto.Description,
                CookingTime = menuItemDto.CookingTime,
                Price = menuItemDto.Price,
                ImagePath = menuItemDto.ImagePath,
                AvailabilityTime = menuItemDto.AvailabilityTime,
                IsAvailable = menuItemDto.IsAvailable,
                CuisineId = menuItemDto.CuisineId,
                MealTypeId = menuItemDto.MealTypeId,
                RestaurantId = manager.RestaurantId,
                Calories = menuItemDto.Calories,
                Proteins = menuItemDto.Proteins,
                Fats = menuItemDto.Fats,
                Carbohydrates = menuItemDto.Carbohydrates,
                TasteInfo = menuItemDto.TasteInfo
            };

            var addedItem = await _menuItemRepository.Add(newItem);
            var itemWithDetails = await _menuItemRepository.GetById(addedItem.Id); // with nav props

            return new CreateMenuItemResponse
            {
                ItemName = itemWithDetails.Name,
                ImagePath = itemWithDetails.ImagePath,
                Description = itemWithDetails.Description,
                CookingTime = itemWithDetails.CookingTime,
                Price = itemWithDetails.Price,
                IsAvailable = itemWithDetails.IsAvailable,
                Cuisine = itemWithDetails.Cuisine?.Name,
                MealType = itemWithDetails.MealType?.Name,
                RestaurantId = itemWithDetails.RestaurantId,
                RestaurantName = itemWithDetails.Restaurant?.RestaurantName
            };
        }


        public async Task<IEnumerable<CreateMenuItemResponse>> GetAvailableMenuItems()
        {
            var menuItems = await _menuItemRepository.GetAll();

            return menuItems
                .Where(m => m.IsAvailable)
                .Select(m => new CreateMenuItemResponse
                {
                    Id = m.Id,
                    ItemName = m.Name,
                    ImagePath = m.ImagePath,
                    Description = m.Description,
                    CookingTime = m.CookingTime,
                    Price = m.Price,
                    IsAvailable = m.IsAvailable,
                    Cuisine = m.Cuisine?.Name,
                    MealType = m.MealType?.Name,
                    RestaurantId = m.RestaurantId,
                    RestaurantName = m.Restaurant?.RestaurantName
                })
                .ToList();
        }

        public async Task<IEnumerable<CreateMenuItemResponse>> GetAvailableMenuItems(int managerId)
        {
            // Get the manager's restaurant ID
            var manager = await _managerRepository.GetById(managerId);
            if (manager == null) return new List<CreateMenuItemResponse>();

            var restaurantId = manager.RestaurantId;

            // Get only menu items for that restaurant
            var menuItems = await _menuItemRepository.GetAll();

            return menuItems
                .Where(m => m.RestaurantId == restaurantId)
                .Select(m => new CreateMenuItemResponse
                {
                    Id = m.Id,
                    ItemName = m.Name,
                    ImagePath = m.ImagePath,
                    Description = m.Description,
                    CookingTime = m.CookingTime,
                    Price = m.Price,
                    IsAvailable = m.IsAvailable,
                    Cuisine = m.Cuisine?.Name,
                    MealType = m.MealType?.Name,
                    RestaurantId = m.RestaurantId,
                    RestaurantName = m.Restaurant?.RestaurantName,
                    tasteInfo = m.TasteInfo
                })
                .ToList();
        }

        public async Task<CreateMenuItemResponse> UpdateMenuItem(int menuItemId, MenuItemUpdate menuItemDto)
        {
            var menuItem = await _menuItemRepository.GetById(menuItemId);
            if (menuItem == null) return null;

            // Update fields
            menuItem.Name = menuItemDto.Name;
            menuItem.Description = menuItemDto.Description;
            menuItem.CookingTime = menuItemDto.CookingTime;
            menuItem.Price = menuItemDto.Price;
            menuItem.ImagePath = menuItemDto.ImagePath;
            menuItem.AvailabilityTime = menuItemDto.AvailabilityTime;
            menuItem.IsAvailable = menuItemDto.IsAvailable;
            menuItem.CuisineId = menuItemDto.CuisineId;
            menuItem.MealTypeId = menuItemDto.MealTypeId;
            menuItem.Calories = menuItemDto.Calories;
            menuItem.Proteins = menuItemDto.Proteins;
            menuItem.Fats = menuItemDto.Fats;
            menuItem.Carbohydrates = menuItemDto.Carbohydrates;
            menuItem.TasteInfo = menuItemDto.TasteInfo;

            await _menuItemRepository.Update(menuItemId, menuItem);

            // Get updated entity with related properties
            var updatedItem = await _menuItemRepository.GetById(menuItemId);

            return new CreateMenuItemResponse
            {
                Id = updatedItem.Id,
                ItemName = updatedItem.Name,
                ImagePath = updatedItem.ImagePath,
                Description = updatedItem.Description,
                CookingTime = updatedItem.CookingTime,
                Price = updatedItem.Price,
                IsAvailable = updatedItem.IsAvailable,
                MealType = updatedItem.MealType.Name,
                Cuisine = updatedItem.Cuisine.Name,
                RestaurantId = updatedItem.RestaurantId,
                RestaurantName = updatedItem.Restaurant?.RestaurantName
            };
        }

        public async Task<bool> DeleteMenuItem(int menuItemId)
        {
            var menuItem = await _menuItemRepository.GetById(menuItemId);
            if (menuItem == null) return false;

            await _menuItemRepository.Delete(menuItemId);
            return true;
        }
        public async Task<MenuItem> SetAvailability(int menuItemId, bool isAvailable)
        {
            var menuItem = await _menuItemRepository.GetById(menuItemId);
            if (menuItem == null)
            {
                throw new Exception($"MenuItem with ID {menuItemId} not found.");
            }

            menuItem.IsAvailable = isAvailable;
            return await _menuItemRepository.Update(menuItemId, menuItem);
        }

        //public async Task<bool> SetAvailabilityByCurrentTime(int menuItemId)
        //{
        //    var menuItem = await _menuItemRepository.GetById(menuItemId);
        //    if (menuItem == null) return false;

        //    var currentTime = DateTime.Now.TimeOfDay;

        //    // Parse the AvailabilityTime string into start and end times
        //    var availabilityTimes = menuItem.AvailabilityTime.Split(" - ");
        //    if (availabilityTimes.Length != 2) return false; // Invalid format

        //    // Convert 12-hour time format to 24-hour time format
        //    var startTime = ParseToTimeSpan(availabilityTimes[0]);
        //    var endTime = ParseToTimeSpan(availabilityTimes[1]);

        //    // Check if the current time falls within the specified time range
        //    menuItem.IsAvailable = currentTime >= startTime && currentTime <= endTime;

        //    await _menuItemRepository.Update(menuItemId, menuItem);
        //    return true;
        //}

        //private TimeSpan ParseToTimeSpan(string time)
        //{
        //    // Convert the 12-hour time format with AM/PM to a 24-hour TimeSpan
        //    var dateTime = DateTime.ParseExact(time, "hh:mm tt", CultureInfo.InvariantCulture);
        //    return dateTime.TimeOfDay;
        //}

        public async Task<bool> SetAvailabilityForAllMenuItems()
        {
            var allMenuItems = await _menuItemRepository.GetAll();
            if (allMenuItems == null || !allMenuItems.Any()) return false;

            var currentTime = DateTime.Now.TimeOfDay;

            foreach (var menuItem in allMenuItems)
            {
                if (string.IsNullOrWhiteSpace(menuItem.AvailabilityTime)) continue;

                var availabilityTimes = menuItem.AvailabilityTime.Split(" - ");
                if (availabilityTimes.Length != 2) continue;

                var startTime = ParseToTimeSpan(availabilityTimes[0]);
                var endTime = ParseToTimeSpan(availabilityTimes[1]);

                menuItem.IsAvailable = currentTime >= startTime && currentTime <= endTime;

                // Replace "Id" with your actual MenuItem ID property if needed
                await _menuItemRepository.Update(menuItem.Id, menuItem);
            }

            return true;
        }

        private TimeSpan ParseToTimeSpan(string time)
        {
            var dateTime = DateTime.ParseExact(time.Trim(), "hh:mm tt", CultureInfo.InvariantCulture);
            return dateTime.TimeOfDay;
        }


        public async Task<CreateMenuItemResponse> GetMenuItemById(int menuItemId)
        {
            try
            {
                var menuItem = await _menuItemRepository.GetById(menuItemId);
                if (menuItem == null)
                    return null;

                return new CreateMenuItemResponse
                {
                    ItemName = menuItem.Name,
                    Description = menuItem.Description,
                    CookingTime = menuItem.CookingTime,
                    Price = menuItem.Price,
                    IsAvailable = menuItem.IsAvailable,
                    Cuisine = menuItem.Cuisine?.Name,
                    MealType = menuItem.MealType?.Name,
                    RestaurantId = menuItem.RestaurantId,
                    RestaurantName = menuItem.Restaurant?.RestaurantName
                };
            }
            catch (Exception)
            {
                throw new Exception($"Failed to fetch MenuItem with ID {menuItemId}");
            }
        }


        public async Task<IEnumerable<CreateMenuItemResponse>> GetMenuItemsByName(string name)
        {
            var menuItems = await _menuItemRepository.GetAll();

            return menuItems
                .Where(m => m.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .Select(m => new CreateMenuItemResponse
                {
                    ItemName = m.Name,
                    Description = m.Description,
                    CookingTime = m.CookingTime,
                    Price = m.Price,
                    IsAvailable = m.IsAvailable,
                    Cuisine = m.Cuisine?.Name,
                    MealType = m.MealType?.Name,
                    RestaurantId = m.RestaurantId,
                    RestaurantName = m.Restaurant?.RestaurantName
                })
                .ToList();
        }

        public async Task<IEnumerable<CreateMenuItemResponse>> GetMenuItemsByCuisine(string cuisineName)
        {
            var menuItems = await _menuItemRepository.GetAll();

            return menuItems
                .Where(m => m.Cuisine != null &&
                            m.Cuisine.Name.Equals(cuisineName, StringComparison.OrdinalIgnoreCase))
                .Select(m => new CreateMenuItemResponse
                {
                    ItemName = m.Name,
                    Description = m.Description,
                    CookingTime = m.CookingTime,
                    Price = m.Price,
                    IsAvailable = m.IsAvailable,
                    Cuisine = m.Cuisine?.Name,
                    MealType = m.MealType?.Name,
                    RestaurantId = m.RestaurantId,
                    RestaurantName = m.Restaurant?.RestaurantName
                })
                .ToList();
        }

        public async Task<IEnumerable<CreateMenuItemResponse>> GetMenuItemsByMealType(string mealTypeName)
        {
            var menuItems = await _menuItemRepository.GetAll();

            return menuItems
                .Where(m => m.MealType != null &&
                            m.MealType.Name.Equals(mealTypeName, StringComparison.OrdinalIgnoreCase))
                .Select(m => new CreateMenuItemResponse
                {
                    ItemName = m.Name,
                    Description = m.Description,
                    CookingTime = m.CookingTime,
                    Price = m.Price,
                    IsAvailable = m.IsAvailable,
                    Cuisine = m.Cuisine?.Name,
                    MealType = m.MealType?.Name,
                    RestaurantId = m.RestaurantId,
                    RestaurantName = m.Restaurant?.RestaurantName
                })
                .ToList();
        }

        public async Task<IEnumerable<CreateMenuItemResponse>> SearchMenuItems(string itemname, string cuisineName, string mealTypeName)
        {
            var menuItems = await _menuItemRepository.GetAll();

            var filteredItems = menuItems.Where(m =>
                (string.IsNullOrEmpty(itemname) || m.Name.Contains(itemname, StringComparison.OrdinalIgnoreCase)) ||
                (string.IsNullOrEmpty(cuisineName) || (m.Cuisine != null && m.Cuisine.Name.Equals(cuisineName, StringComparison.OrdinalIgnoreCase))) ||
                (string.IsNullOrEmpty(mealTypeName) || (m.MealType != null && m.MealType.Name.Equals(mealTypeName, StringComparison.OrdinalIgnoreCase)))
            );

            return filteredItems
                .Select(m => new CreateMenuItemResponse
                {
                    ItemName = m.Name,
                    Description = m.Description,
                    CookingTime = m.CookingTime,
                    Price = m.Price,
                    IsAvailable = m.IsAvailable,
                    Cuisine = m.Cuisine?.Name,
                    MealType = m.MealType?.Name,
                    RestaurantId = m.RestaurantId,
                    RestaurantName = m.Restaurant?.RestaurantName
                })
                .ToList();
        }

        public async Task<IEnumerable<CreateMenuItemResponse>> FilterMenuItems(decimal? minPrice,decimal? maxPrice,bool? isAvailable,string? cuisineName,string? mealTypeName,string? sortBy,string? sortOrder)
        {
            var menuItems = await _menuItemRepository.GetAll();

            // Apply filters
            if (minPrice.HasValue)
                menuItems = menuItems.Where(m => m.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                menuItems = menuItems.Where(m => m.Price <= maxPrice.Value);

            if (isAvailable.HasValue)
                menuItems = menuItems.Where(m => m.IsAvailable == isAvailable.Value);

            if (!string.IsNullOrEmpty(cuisineName))
                menuItems = menuItems.Where(m => m.Cuisine?.Name != null && m.Cuisine.Name.Equals(cuisineName, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(mealTypeName))
                menuItems = menuItems.Where(m => m.MealType?.Name != null && m.MealType.Name.Equals(mealTypeName, StringComparison.OrdinalIgnoreCase));

            // Sorting logic
            sortBy = sortBy?.ToLower();
            sortOrder = sortOrder?.ToLower();

            menuItems = sortBy switch
            {
                "price" => sortOrder == "desc" ? menuItems.OrderByDescending(m => m.Price) : menuItems.OrderBy(m => m.Price),
                "name" or "itemname" => sortOrder == "desc" ? menuItems.OrderByDescending(m => m.Name) : menuItems.OrderBy(m => m.Name),
                _ => menuItems.OrderBy(m => m.Name) // Default sort
            };

            // Mapping to DTO
            var response = menuItems.Select(m => new CreateMenuItemResponse
            {
                ItemName = m.Name,
                Description = m.Description,
                CookingTime = m.CookingTime,
                Price = m.Price,
                IsAvailable = m.IsAvailable,
                Cuisine = m.Cuisine?.Name,
                MealType = m.MealType?.Name,
                RestaurantId = m.RestaurantId,
                RestaurantName = m.Restaurant?.RestaurantName
            });

            return response;
        }

        public async Task<IEnumerable<CreateResturantManagerResponse>> GetAllManagers()
        {
            var managers = await _managerRepository.GetAll();

            if (managers == null)
                throw new Exception("No managers found");

            var responseList = new List<CreateResturantManagerResponse>();

            foreach (var manager in managers)
            {
                var restaurant = await _restaurantRepository.GetById(manager.RestaurantId);

                var response = new CreateResturantManagerResponse
                {
                    Id = manager.ManagerId,
                    FullName = manager.Username,
                    Email = manager.Email,
                    Phone = manager.PhoneNumber,
                    RestaurantName = restaurant?.RestaurantName
                };

                responseList.Add(response);
            }

            return responseList;
        }


        public async Task<int> GetTotalManagers()
        {
            return await _context.RestaurantManagers.CountAsync();
        }


    }
}
