using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;


namespace HotPotAPI.Interfaces
{
    public interface ICustomerService
    {
        Task<CreateCustomerResponse> AddCustomer(CreateCustomerRequest request);

        // Restaurant-related
        Task<List<Restaurant>> GetAllRestaurants();
        Task<Restaurant> GetRestaurantByName(string name);

        // Menu-related
        Task<List<MenuItem>> GetMenuByRestaurantName(string name);
        Task<List<CreateMenuItemResponse>> GetAvailableMenuItems();
        Task<List<MenuItem>> GetMenuByRestaurantId(int restaurantId);
        Task<List<CreateMenuItemResponse>> GetMenuItemsByName(string name);
        Task<List<CreateMenuItemResponse>> GetMenuItemsByCuisine(string cuisineName);
        Task<List<CreateMenuItemResponse>> GetMenuItemsByMealType(string mealTypeName);

        //Advanced Search & Filter
        Task<List<CreateMenuItemResponse>> SearchMenuItems(string? itemname, string? cuisineName, string? mealTypeName);
        Task<IEnumerable<CreateMenuItemResponse>> FilterMenuItems(decimal? minPrice, decimal? maxPrice, bool? isAvailable, string? cuisineName, string? mealTypeName, string? sortBy, string? sortOrder);

        // Cart-related methods
        Task<CartResponse> GetCartByCustomerId(int customerId);
        Task<bool> ClearCart(int customerId);

        //cartitem-related methods
        Task<CreateCartItemResponse> AddCartItem(int customerId, CreateCartItemRequest cartItemCreate);
        Task<CreateCartItemResponse> UpdateCartItem(int customerId, int cartItemId, CartItemUpdate cartItemUpdate);
        Task<bool> RemoveCartItem(int customerId, int cartItemId);


        Task<int> GetTotalCustomers();
        Task<List<Cuisine>> GetAllCuisines();
        Task<List<MealType>> GetAllMealTypes();
        Task<CustomerAddress> AddCustomerAddress(int customerId, AddCustomerAddressRequest request);
        Task<IEnumerable<CustomerAddress>> GetAddressesByCustomerId(int customerId);
        Task<CustomerWithAddress?> GetCustomerWithAddressById(int id);

    }
}
