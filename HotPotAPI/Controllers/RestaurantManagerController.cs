using HotPotAPI.Exceptions;
using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotPotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantManagerController : ControllerBase
    {
        private readonly IRestaurantManagerService _service;

        private readonly IOrderService _orderService;

        public RestaurantManagerController(IRestaurantManagerService service, IOrderService orderService)
        {
            _service = service;
            _orderService = orderService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register")]
        public async Task<ActionResult<CreateResturantManagerResponse>> AddRestaurantManager(CreateRestaurantManagerRequest request)
        {
            try
            {
                var result = await _service.AddRestaurantManager(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.InnerException?.Message ?? ex.Message);
            }
        }

        [HttpPost("manager/{managerId}/menuitems")]
        public async Task<ActionResult<CreateMenuItemResponse>> AddMenuItem(int managerId, [FromBody] CreateMenuItemRequest menuItemDto)
        {
            try
            {
                var newMenuItem = await _service.AddMenuItem(managerId, menuItemDto);
                return CreatedAtAction(nameof(GetMenuItemById), new { menuItemId = newMenuItem.RestaurantId }, newMenuItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut("UpdateManagerById/{id}")]
        public async Task<ActionResult<CreateResturantManagerResponse>> UpdateManagerById(int id, [FromBody] RestaurantManagerUpdate request)
        {
            try
            {
                var updatedManager = await _service.UpdateManagerById(id, request);
                return Ok(updatedManager);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Manager not found");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("manager/{managerId}")]
        public async Task<IActionResult> DeleteManagerById(int managerId)
        {
            var result = await _service.DeleteManagerById(managerId);

            if (result)
            {
                return Ok(new { Message = "Restaurant manager deleted successfully." });
            }
            return NotFound(new { Message = "Restaurant manager not found." });
        }

        [HttpGet("GetManagerById/{managerId}")]
        public async Task<IActionResult> GetManagerById(int managerId)
        {
            var manager = await _service.GetManagerById(managerId);
            return Ok(manager);
        }


        [HttpGet("GetAllMenuItems")]
        public async Task<ActionResult<IEnumerable<CreateMenuItemResponse>>> GetAllMenuItems()
        {
            try
            {
                var menuItems = await _service.GetAvailableMenuItems();
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpGet("GetAllMenuItems/{managerId}")]
        public async Task<ActionResult<IEnumerable<CreateMenuItemResponse>>> GetAllMenuItems(int managerId)
        {
            try
            {
                var menuItems = await _service.GetAvailableMenuItems(managerId);
                if (!menuItems.Any())
                {
                    return NotFound(new { Message = "No menu items found for this manager's restaurant." });
                }
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("{menuItemId}")]
        public async Task<ActionResult<CreateMenuItemResponse>> GetMenuItemById(int menuItemId)
        {
            try
            {
                var menuItem = await _service.GetMenuItemById(menuItemId);
                if (menuItem == null)
                    return NotFound($"MenuItem with ID {menuItemId} not found.");

                return Ok(menuItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("name/{menuName}")]
        public async Task<ActionResult<IEnumerable<CreateMenuItemResponse>>> GetMenuItemsByName(string menuName)
        {
            try
            {
                var menuItems = await _service.GetMenuItemsByName(menuName);

                if (menuItems == null || !menuItems.Any())
                    return NotFound($"No menu items found with name containing '{menuName}'.");

                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("menu-by-cuisine/{cuisineName}")]
        public async Task<ActionResult<IEnumerable<CreateMenuItemResponse>>> GetMenuItemsByCuisine(string cuisineName)
        {
            try
            {
                var items = await _service.GetMenuItemsByCuisine(cuisineName);
                if (items == null || !items.Any())
                    return NotFound($"No menu items found for cuisine '{cuisineName}'.");

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("menu-by-mealtype/{mealTypeName}")]
        public async Task<ActionResult<IEnumerable<CreateMenuItemResponse>>> GetMenuItemsByMealType(string mealTypeName)
        {
            try
            {
                var items = await _service.GetMenuItemsByMealType(mealTypeName);
                if (items == null || !items.Any())
                    return NotFound($"No menu items found for meal type '{mealTypeName}'.");

                return Ok(items);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpPut("{menuItemId}/availability/manual")]
        public async Task<ActionResult> UpdateMenuItemAvailability(int menuItemId, bool isAvailable)
        {
            try
            {
                var updatedItem = await _service.SetAvailability(menuItemId, isAvailable);
                return Ok(new { Message = "Availability updated successfully", Data = updatedItem });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        //[HttpPut("{menuItemId}/availability/auto")]
        //public async Task<ActionResult> SetAvailability(int menuItemId)
        //{
        //    try
        //    {
        //        var result = await _service.SetAvailabilityByCurrentTime(menuItemId);
        //        return result ? Ok(new { Message = "Availability updated successfully by current time." }) : NotFound($"MenuItem with ID {menuItemId} not found.");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = ex.Message });
        //    }
        //}

        [HttpPut("availability/auto")]
        public async Task<ActionResult> SetAvailabilityForAll()
        {
            try
            {
                var result = await _service.SetAvailabilityForAllMenuItems();
                return result ? Ok(new { Message = "Availability updated for all menu items." })
                              : NotFound("No menu items found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }


        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CreateMenuItemResponse>>> SearchMenuItems([FromQuery] string? itemname,[FromQuery] string? cuisineName,[FromQuery] string? mealTypeName)
        {
            try
            {
                var menuItems = await _service.SearchMenuItems(itemname, cuisineName, mealTypeName);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<CreateMenuItemResponse>>> FilterMenuItems(
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool? isAvailable,
        [FromQuery] string? cuisineName,
        [FromQuery] string? mealTypeName,
        [FromQuery] string? sortBy,
        [FromQuery] string? sortOrder)
        {
            var filteredItems = await _service.FilterMenuItems(
                minPrice,
                maxPrice,
                isAvailable,
                cuisineName,
                mealTypeName,
                sortBy,
                sortOrder
            );

            return Ok(filteredItems);
        }

        [HttpDelete("{menuItemId}")]
        public async Task<ActionResult> DeleteMenuItem(int menuItemId)
        {
            try
            {
                var result = await _service.DeleteMenuItem(menuItemId);
                return result ? NoContent() : NotFound($"MenuItem with ID {menuItemId} not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPut("update/{menuItemId}")]
        public async Task<IActionResult> UpdateMenuItem(int menuItemId, [FromBody] MenuItemUpdate menuItemDto)
        {
            if (menuItemDto == null)
                return BadRequest("Menu item data is required.");

            var updatedItem = await _service.UpdateMenuItem(menuItemId, menuItemDto);

            if (updatedItem == null)
                return NotFound($"Menu item with ID {menuItemId} not found.");

            return Ok(updatedItem);
        }

        // GET: api/RestaurantManager/{managerId}/orders
        [HttpGet("{managerId}/orders")]
        public async Task<IActionResult> GetOrdersByManager(int managerId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByRestaurantManager(managerId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, $"An error occurred while fetching orders: {ex.Message}");
            }
        }

        //[HttpPut("restaurant/update-status/{orderId}")]
        //public async Task<IActionResult> UpdateOrderStatusByRestaurant(int orderId, [FromBody] int statusId)
        //{
        //    try
        //    {
        //        var order = await _orderService.GetOrderById(orderId);
        //        if (order == null) return NotFound("Order not found");

        //        await _orderService.UpdateOrderStatusByRestaurant(orderId, statusId);
        //        return Ok("Order status updated by restaurant");
        //    }
        //    catch (Exception ex)
        //    {
        //        // Optionally log the exception here
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}


        // PUT: api/RestaurantManager/{managerId}/orders/{orderId}/status/{statusId}
        [HttpPut("{managerId}/orders/{orderId}/status/{statusId}")]
        public async Task<IActionResult> UpdateOrderStatus(int managerId, int orderId, int statusId)
        {
            try
            {
                await _orderService.UpdateOrderStatusByRestaurant(orderId, statusId, managerId);
                return NoContent(); // 204 No Content
            }
            catch (DeliveryPartnerUnavailableException ex)
            {
                return StatusCode(400, ex.Message);
            }
            catch (Exception ex)
            {
                // Log the exception here if needed
                return StatusCode(500, $"An error occurred while updating order status: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllManagers")]
        public async Task<IActionResult> GetAllManagers()
        {
            var managers = await _service.GetAllManagers();
            return Ok(managers);
        }

    }
}
