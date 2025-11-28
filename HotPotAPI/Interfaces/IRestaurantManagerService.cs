using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;

namespace HotPotAPI.Interfaces
{
    public interface IRestaurantManagerService
    {
        Task<CreateResturantManagerResponse> AddRestaurantManager(CreateRestaurantManagerRequest request);
        Task<CreateResturantManagerResponse> UpdateManagerById(int managerId, RestaurantManagerUpdate request);
        Task<bool> DeleteManagerById(int managerId);
        Task<CreateResturantManagerResponse> GetManagerById(int managerId);
        Task<IEnumerable<CreateResturantManagerResponse>> GetAllManagers();
        Task<CreateMenuItemResponse> AddMenuItem(int managerId, CreateMenuItemRequest menuItemDto);
        Task<IEnumerable<CreateMenuItemResponse>> GetAvailableMenuItems();
        Task<IEnumerable<CreateMenuItemResponse>> GetAvailableMenuItems(int managerId);
        Task<IEnumerable<CreateMenuItemResponse>> GetMenuItemsByName(string name);
        Task<IEnumerable<CreateMenuItemResponse>> GetMenuItemsByCuisine(string cuisineName);
        Task<IEnumerable<CreateMenuItemResponse>> GetMenuItemsByMealType(string mealTypeName);
        Task<CreateMenuItemResponse> UpdateMenuItem(int menuItemId, MenuItemUpdate menuItemDto);
        Task<bool> DeleteMenuItem(int menuItemId);
        Task<CreateMenuItemResponse> GetMenuItemById(int menuItemId);
        Task<MenuItem> SetAvailability(int menuItemId, bool isAvailable);

        //Task<bool> SetAvailabilityByCurrentTime(int menuItemId);

        Task<bool> SetAvailabilityForAllMenuItems();
        Task<IEnumerable<CreateMenuItemResponse>> SearchMenuItems(string? itemname, string? cuisineName, string? mealTypeName);
        Task<IEnumerable<CreateMenuItemResponse>> FilterMenuItems(decimal? minPrice, decimal? maxPrice, bool? isAvailable, string? cuisineName, string? mealTypeName, string? sortBy, string? sortOrder);
        Task<int> GetTotalManagers();

    }
}
