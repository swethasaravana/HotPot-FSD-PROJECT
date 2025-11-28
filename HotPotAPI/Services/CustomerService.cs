using HotPotAPI.Contexts;
using HotPotAPI.Exceptions;
using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace HotPotAPI.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly HotPotDbContext _context;
        private readonly IRepository<string, User> _userRepository;
        private readonly IRepository<int, Customer> _customerRepository;
        private readonly IRepository<int, Restaurant> _restaurantRepository;
        private readonly IRepository<int, MenuItem> _menuItemRepository;
        private readonly IRepository<int, Cart> _cartRepository;
        private readonly IRepository<int, CartItem> _cartItemRepository;

        public CustomerService(HotPotDbContext context,
                              IRepository<string, User> userRepository, IRepository<int, Customer> customerRepository,
                              IRepository<int, Restaurant> restaurantRepository,IRepository<int, MenuItem> menuItemRepository,
                              IRepository<int, Cart> cartRepository,IRepository<int, CartItem> cartItemRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _customerRepository = customerRepository;
            _restaurantRepository = restaurantRepository;
            _menuItemRepository = menuItemRepository;
            _cartRepository = cartRepository;
            _cartItemRepository = cartItemRepository;
            
        }
        public async Task<CreateCustomerResponse> AddCustomer(CreateCustomerRequest request)
        {
            HMACSHA512 hmac = new HMACSHA512();
            byte[] passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            var user = MapCustomerToUser(request, passwordHash, hmac.Key);

            try
            {
                var userResult = await _userRepository.Add(user);
                if (userResult == null)
                    throw new Exception("Failed to create user");
            }
            catch (Exception ex)
            {
                if (ex.InnerException?.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase) == true ||
                    ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase))
                {
                    throw new EmailAlreadyExistsException(request.Email);
                }

                throw; 
            }

            var customer = MapCustomer(request);
            customer.User = user; 
            var customerResult = await _customerRepository.Add(customer);
            if (customerResult == null)
                throw new Exception("Failed to create customer");

            return new CreateCustomerResponse { Id = customerResult.Id };
        }

        private Customer MapCustomer(CreateCustomerRequest request)
        {
            return new Customer
            {
                Name = request.Name,
                Gender = request.Gender,
                Email = request.Email,
                Phone = request.Phone,
                Addresses = request.Addresses?.Select(addr => new CustomerAddress
                {
                    Label = addr.Label,
                    Street = addr.Street,
                    City = addr.City,
                    Pincode = addr.Pincode
                }).ToList()
            };
        }

        private User MapCustomerToUser(CreateCustomerRequest request, byte[] passwordHash, byte[] key)
        {
            return new User
            {
                Username = request.Email,
                Password = passwordHash,
                HashKey = key,
                Role = "Customer"
            };
        }
        public async Task<IEnumerable<CustomerAddress>> GetAddressesByCustomerId(int customerId)
        {
            return await _context.CustomerAddresses
                .Where(a => a.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<CustomerAddress> AddCustomerAddress(int customerId, AddCustomerAddressRequest request)
        {
            var address = new CustomerAddress
            {
                CustomerId = customerId,
                Label = request.Label,
                Street = request.Street,
                City = request.City,
                Pincode = request.Pincode
            };

            _context.CustomerAddresses.Add(address);
            await _context.SaveChangesAsync();

            return address;
        }

        public async Task<CustomerWithAddress?> GetCustomerWithAddressById(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Addresses)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null) return null;

            return new CustomerWithAddress
            {
                Name = customer.Name,
                Gender = customer.Gender,
                Email = customer.Email,
                Phone = customer.Phone,
                Addresses = customer.Addresses.Select(a => new AddressDTO
                {
                    Label = a.Label,
                    Street = a.Street,
                    City = a.City,
                    Pincode = a.Pincode
                }).ToList()
            };
        }

        public async Task<List<Restaurant>> GetAllRestaurants()
        {
            var restaurants = await _restaurantRepository.GetAll();
            return restaurants.ToList();
        }

        public async Task<Restaurant> GetRestaurantByName(string name)
        {
            var allRestaurants = await _restaurantRepository.GetAll();
            return allRestaurants.FirstOrDefault(r => r.RestaurantName.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<List<MenuItem>> GetMenuByRestaurantName(string name)
        {
            var allRestaurants = await _restaurantRepository.GetAll();
            var restaurant = allRestaurants.FirstOrDefault(r =>
                r.RestaurantName.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (restaurant == null)
                throw new Exception("Restaurant not found.");

            var allMenus = await _menuItemRepository.GetAll();
            var menuItems = allMenus
                .Where(m => m.RestaurantId == restaurant.RestaurantId)
                .ToList();

            return menuItems;
        }
        public async Task<List<CreateMenuItemResponse>> GetAvailableMenuItems()
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

        public async Task<List<MenuItem>> GetMenuByRestaurantId(int restaurantId)
        {
            var restaurant = await _restaurantRepository.GetById(restaurantId);

            if (restaurant == null)
                throw new Exception("Restaurant not found.");

            var allMenus = await _menuItemRepository.GetAll();
            var menuItems = allMenus
                .Where(m => m.RestaurantId == restaurant.RestaurantId)
                .ToList();

            return menuItems;
        }

        public async Task<List<CreateMenuItemResponse>> GetMenuItemsByName(string name)
        {
            var menuItems = await _menuItemRepository.GetAll();

            return menuItems
                .Where(m => m.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                .Select(m => new CreateMenuItemResponse
                {
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

        public async Task<List<CreateMenuItemResponse>> GetMenuItemsByCuisine(string cuisineName)
        {
            var menuItems = await _menuItemRepository.GetAll();

            return menuItems
                .Where(m => m.Cuisine != null &&
                            m.Cuisine.Name.Equals(cuisineName, StringComparison.OrdinalIgnoreCase))
                .Select(m => new CreateMenuItemResponse
                {
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

        public async Task<List<CreateMenuItemResponse>> GetMenuItemsByMealType(string mealTypeName)
        {
            var menuItems = await _menuItemRepository.GetAll();

            return menuItems
                .Where(m => m.MealType != null &&
                            m.MealType.Name.Equals(mealTypeName, StringComparison.OrdinalIgnoreCase))
                .Select(m => new CreateMenuItemResponse
                {
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

        public async Task<List<CreateMenuItemResponse>> SearchMenuItems(string? itemname, string? cuisineName, string? mealTypeName)
        {
            var menuItems = await _menuItemRepository.GetAll();

            var filteredItems = menuItems.Where(m =>
                (string.IsNullOrEmpty(itemname) || m.Name.Contains(itemname, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(cuisineName) || (m.Cuisine != null && m.Cuisine.Name.Equals(cuisineName, StringComparison.OrdinalIgnoreCase))) &&
                (string.IsNullOrEmpty(mealTypeName) || (m.MealType != null && m.MealType.Name.Equals(mealTypeName, StringComparison.OrdinalIgnoreCase)))
            );

            return filteredItems
                .Select(m => new CreateMenuItemResponse
                {
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

        public async Task<IEnumerable<CreateMenuItemResponse>> FilterMenuItems(decimal? minPrice, decimal? maxPrice, bool? isAvailable, string? cuisineName, string? mealTypeName, string? sortBy, string? sortOrder)
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
                ImagePath = m.ImagePath,
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

        public async Task<CartResponse> GetCartByCustomerId(int customerId)
        {
            var cart = (await _cartRepository.GetAll())
                .FirstOrDefault(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow,
                    CartItems = new List<CartItem>()
                };
                cart = await _cartRepository.Add(cart);
            }

            var cartItems = (await _cartItemRepository.GetAll())
                .Where(ci => ci.CartId == cart.CartId)
                .ToList();

            var response = new CartResponse
            {
                CartId = cart.CartId,
                CustomerId = cart.CustomerId,
                CreatedAt = cart.CreatedAt,
                Items = new List<CreateCartItemResponse>()
            };

            foreach (var item in cartItems)
            {
                var menuItem = await _menuItemRepository.GetById(item.MenuItemId);
                response.Items.Add(new CreateCartItemResponse
                {
                    CartItemId = item.CartItemId,
                    MenuItemId = item.MenuItemId,
                    MenuItemName = menuItem?.Name,
                    Quantity = item.Quantity,
                    PriceAtPurchase = item.PriceAtPurchase
                });
            }

            response.TotalPrice = response.Items.Sum(i => i.TotalPrice);
            return response;
        }

        public async Task<bool> ClearCart(int customerId)
        {
            var cart = (await _cartRepository.GetAll())
                .FirstOrDefault(c => c.CustomerId == customerId);

            if (cart == null) return false;

            var cartItems = (await _cartItemRepository.GetAll())
                .Where(ci => ci.CartId == cart.CartId);

            foreach (var item in cartItems)
            {
                await _cartItemRepository.Delete(item.CartItemId);
            }

            return true;
        }

        public async Task<CreateCartItemResponse> AddCartItem(int customerId, CreateCartItemRequest cartItemCreate)
        {
            // Get the cart for the customer
            var cart = (await _cartRepository.GetAll())
                .FirstOrDefault(c => c.CustomerId == customerId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CustomerId = customerId,
                    CreatedAt = DateTime.UtcNow
                };
                cart = await _cartRepository.Add(cart);
            }

            // Get the menu item to be added
            var menuItem = await _menuItemRepository.GetById(cartItemCreate.MenuItemId);
            if (menuItem == null) throw new Exception("MenuItem not found");

            // Check if cart has items from another restaurant
            var existingCartItems = (await _cartItemRepository.GetAll())
                .Where(ci => ci.CartId == cart.CartId)
                .ToList();

            if (existingCartItems.Any())
            {
                // Get the restaurantId of the first existing item
                var existingItem = await _menuItemRepository.GetById(existingCartItems.First().MenuItemId);
                if (existingItem.RestaurantId != menuItem.RestaurantId)
                {
                    throw new Exception("You can only add items from only one restaurant at a time.");
                }
            }

            // Check if the menu item already exists in the cart
            var existingCartItem = existingCartItems
                .FirstOrDefault(ci => ci.MenuItemId == cartItemCreate.MenuItemId);

            if (existingCartItem != null)
            {
                // If the item already exists, increase the quantity
                existingCartItem.Quantity += cartItemCreate.Quantity;
                var updatedItem = await _cartItemRepository.Update(existingCartItem.CartItemId, existingCartItem);

                return new CreateCartItemResponse
                {
                    CartItemId = updatedItem.CartItemId,
                    MenuItemId = updatedItem.MenuItemId,
                    MenuItemName = menuItem.Name,
                    Quantity = updatedItem.Quantity,
                    PriceAtPurchase = updatedItem.PriceAtPurchase
                };
            }
            else
            {
                // If the item doesn't exist, add it as a new item
                var cartItem = new CartItem
                {
                    CartId = cart.CartId,
                    MenuItemId = cartItemCreate.MenuItemId,
                    Quantity = cartItemCreate.Quantity,
                    PriceAtPurchase = menuItem.Price
                };

                var addedItem = await _cartItemRepository.Add(cartItem);

                return new CreateCartItemResponse
                {
                    CartItemId = addedItem.CartItemId,
                    MenuItemId = addedItem.MenuItemId,
                    MenuItemName = menuItem.Name,
                    Quantity = addedItem.Quantity,
                    PriceAtPurchase = addedItem.PriceAtPurchase
                };
            }
        }

        public async Task<CreateCartItemResponse> UpdateCartItem(int customerId, int cartItemId, CartItemUpdate cartItemUpdate)
        {
            var cartItem = await _cartItemRepository.GetById(cartItemId);
            if (cartItem == null)
                throw new Exception("Cart item not found");

            var cart = await _cartRepository.GetById(cartItem.CartId);
            if (cart == null || cart.CustomerId != customerId)
                throw new Exception("Cart does not belong to customer");

            cartItem.Quantity = cartItemUpdate.Quantity;

            var updatedItem = await _cartItemRepository.Update(cartItem.CartItemId, cartItem);

            var menuItem = await _menuItemRepository.GetById(updatedItem.MenuItemId);

            return new CreateCartItemResponse
            {
                CartItemId = updatedItem.CartItemId,
                MenuItemId = updatedItem.MenuItemId,
                MenuItemName = menuItem?.Name ?? "Unknown",
                Quantity = updatedItem.Quantity,
                PriceAtPurchase = updatedItem.PriceAtPurchase
            };
        }

        public async Task<bool> RemoveCartItem(int customerId, int cartItemId)
        {
            var cartItem = await _cartItemRepository.GetById(cartItemId);
            if (cartItem == null) return false;

            var cart = await _cartRepository.GetById(cartItem.CartId);
            if (cart == null || cart.CustomerId != customerId)
                return false;

            await _cartItemRepository.Delete(cartItemId);
            return true;
        }

        public async Task<int> GetTotalCustomers()
        {
            return await _context.Customers.CountAsync();
        }

        public async Task<List<Cuisine>> GetAllCuisines()
        {
            return await _context.Cuisines.ToListAsync();
        }

        public async Task<List<MealType>> GetAllMealTypes()
        {
            return await _context.MealTypes.ToListAsync();
        }

        

    }

}

