using HotPotAPI.Interfaces;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotPotAPI.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IRestaurantManagerService _restaurantManagerService;
        private readonly IDeliveryPartnerService _deliveryPartnerService;

        public AdminController(IAdminService adminService, 
                               IOrderService orderService,
                               ICustomerService customerService,
                               IRestaurantManagerService restaurantManagerService,
                               IDeliveryPartnerService deliveryPartnerService)
        {
            _adminService = adminService;
            _orderService = orderService;
            _customerService = customerService;
            _restaurantManagerService = restaurantManagerService;
            _deliveryPartnerService = deliveryPartnerService;

        }

        [HttpPost("register")]
        public async Task<ActionResult<CreateAdminResponse>> CreateAdmin(CreateAdminRequest request)
        {
            try
            {
                var result = await _adminService.CreateAdmin(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("restaurants")]
        public async Task<IActionResult> AddRestaurant([FromBody] CreateRestaurant dto)
        {
            try
            {
                var added = await _adminService.AddRestaurant(dto);
                return Ok(added);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message} | Inner: {ex.InnerException?.Message}");
            }
        }

        // Update Restaurant
        [Authorize(Roles = "Admin")]
        [HttpPut("restaurants/{id}")]
        public async Task<IActionResult> UpdateRestaurant(int id, [FromBody] CreateRestaurant dto)
        {
            try
            {
                var updated = await _adminService.UpdateRestaurant(id, dto);
                if (updated == null)
                    return NotFound("Restaurant not found.");

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // Delete Restaurant
        [Authorize(Roles = "Admin")]
        [HttpDelete("restaurants/{id}")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            try
            {
                var result = await _adminService.DeleteRestaurant(id);
                if (result)
                    return NoContent();
                return NotFound("Restaurant not found.");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // Get All Restaurants
        [Authorize(Roles = "Admin")]
        [HttpGet("restaurants")]
        public async Task<IActionResult> GetAllRestaurants()
        {
            try
            {
                var restaurants = await _adminService.GetAllRestaurants();
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

        // Get Restaurant by Id
        [HttpGet("restaurants/{id}")]
        public async Task<IActionResult> GetRestaurantById(int id)
        {
            try
            {
                var restaurant = await _adminService.GetRestaurantById(id);
                if (restaurant == null)
                    return NotFound("Restaurant not found.");
                return Ok(restaurant);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // API endpoint to get dashboard stats
        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                // Call services to get the total counts
                var totalCustomers = await _customerService.GetTotalCustomers();
                var totalRestaurants = await _adminService.GetTotalRestaurants();
                var totalRestaurantManagers = await _restaurantManagerService.GetTotalManagers();
                var totalDeliveryPartners = await _deliveryPartnerService.GetTotalDeliveryPartners();

                var stats = new
                {
                    totalCustomers,
                    totalRestaurants,
                    totalRestaurantManagers,
                    totalDeliveryPartners
                };
                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
        }

    }
}
