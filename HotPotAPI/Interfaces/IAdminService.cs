using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;

namespace HotPotAPI.Interfaces
{
    public interface IAdminService
    {
        Task<CreateAdminResponse> CreateAdmin(CreateAdminRequest request);
        Task<Restaurant> AddRestaurant(CreateRestaurant dto);
        Task<Restaurant> UpdateRestaurant(int id, CreateRestaurant dto);
        Task<bool> DeleteRestaurant(int id);
        Task<List<Restaurant>> GetAllRestaurants();
        Task<Restaurant> GetRestaurantById(int id);
        Task<int> GetTotalRestaurants();
    }
}
