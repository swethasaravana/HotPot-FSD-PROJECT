using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotPotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // For sign-up purpose
        [HttpPost("register")]
        public async Task<ActionResult<CreateCustomerResponse>> CreateCustomer(CreateCustomerRequest request)
        {
            try
            {
                var result = await _customerService.AddCustomer(request);
                return Created("", result);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("restaurants")]
        public async Task<IActionResult> GetAllRestaurants()
        {
            try
            {
                var restaurants = await _customerService.GetAllRestaurants();
                return Ok(restaurants);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpGet("restaurants/{name}")]
        public async Task<IActionResult> GetRestaurantByName(string name)
        {
            try
            {
                var restaurant = await _customerService.GetRestaurantByName(name);
                if (restaurant == null) return NotFound("Restaurant not found");
                return Ok(restaurant);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("menus/by-restaurant/{name}")]
        public async Task<IActionResult> GetMenuByRestaurantName(string name)
        {
            try
            {
                var menuItems = await _customerService.GetMenuByRestaurantName(name);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetAllMenuItems")]
        public async Task<ActionResult<IEnumerable<MenuItem>>> GetAllMenuItems()
        {
            try
            {
                var menuItems = await _customerService.GetAvailableMenuItems();
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("menus/by-restaurant-id/{id}")]
        public async Task<IActionResult> GetMenuByRestaurantId(int id)
        {
            try
            {
                var menuItems = await _customerService.GetMenuByRestaurantId(id);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("name/{menuName}")]
        public async Task<ActionResult<List<CreateMenuItemResponse>>> GetMenuItemsByName(string name)
        {
            try
            {
                var menuItems = await _customerService.GetMenuItemsByName(name);
                return Ok(menuItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("menu-by-cuisine/{cuisineName}")]
        public async Task<ActionResult<List<CreateMenuItemResponse>>> GetMenuItemsByCuisine(string cuisineName)
        {
            try
            {
                var items = await _customerService.GetMenuItemsByCuisine(cuisineName);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("menu-by-mealtype/{mealTypeName}")]
        public async Task<ActionResult<List<CreateMenuItemResponse>>> GetMenuItemsByMealType(string mealTypeName)
        {
            try
            {
                var items = await _customerService.GetMenuItemsByMealType(mealTypeName);
                return Ok(items);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<CreateMenuItemResponse>>> SearchMenuItems([FromQuery] string? itemname, [FromQuery] string? cuisineName, [FromQuery] string? mealTypeName)
        {
            try
            {
                var menuItems = await _customerService.SearchMenuItems(itemname, cuisineName, mealTypeName);
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
            var filteredItems = await _customerService.FilterMenuItems(
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

        // GET: api/cart/{customerId}
        [HttpGet("GetCart/{customerId}")]
        public async Task<ActionResult<CartResponse>> GetCart(int customerId)
        {
            var cart = await _customerService.GetCartByCustomerId(customerId);
            if (cart == null) return NotFound("Cart not found");
            return Ok(cart);
        }

        // DELETE: api/cart/{customerId}
        [HttpDelete("ClearCart/{customerId}")]
        public async Task<IActionResult> ClearCart(int customerId)
        {
            var success = await _customerService.ClearCart(customerId);
            if (!success) return NotFound("Cart not found or already empty");
            return NoContent();
        }

        // POST: api/cart/{customerId}/items
        [HttpPost("AddCartItems/{customerId}/items")]
        public async Task<ActionResult<CreateCartItemResponse>> AddCartItem(int customerId, [FromBody] CreateCartItemRequest request)
        {
            try
            {
                var response = await _customerService.AddCartItem(customerId, request);
                return CreatedAtAction(nameof(GetCart), new { customerId = customerId }, response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/cart/{customerId}/items/{cartItemId}
        [HttpPut("UpdateQuantity/{customerId}/items/{cartItemId}")]
        public async Task<ActionResult<CreateCartItemResponse>> UpdateCartItem(int customerId, int cartItemId, CartItemUpdate update)
        {
            var response = await _customerService.UpdateCartItem(customerId, cartItemId, update);
            return Ok(response);
        }

        // DELETE: api/cart/{customerId}/items/{cartItemId}
        [HttpDelete("DeleteCartItems/{customerId}/items/{cartItemId}")]
        public async Task<IActionResult> RemoveCartItem(int customerId, int cartItemId)
        {
            var success = await _customerService.RemoveCartItem(customerId, cartItemId);
            if (!success) return NotFound("Cart item not found or unauthorized");
            return NoContent();
        }

        [HttpGet("cuisines")]
        public async Task<ActionResult<List<Cuisine>>> GetAllCuisines()
        {
            var cuisines = await _customerService.GetAllCuisines();

            if (cuisines == null || cuisines.Count == 0)
            {
                return NotFound("No cuisines found.");
            }

            return Ok(cuisines);
        }


        [HttpGet("customer/{customerId}/addresses")]
        public async Task<IActionResult> GetCustomerAddresses(int customerId)
        {
            try
            {
                var addresses = await _customerService.GetAddressesByCustomerId(customerId);
                return Ok(addresses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpPost("customer/{customerId}/add-address")]
        public async Task<IActionResult> AddCustomerAddress(int customerId, [FromBody] AddCustomerAddressRequest request)
        {
            try
            {
                var address = await _customerService.AddCustomerAddress(customerId, request);
                return Ok(address);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("with-address/{id}")]
        public async Task<ActionResult<CustomerWithAddress>> GetCustomerWithAddress(int id)
        {
            var customer = await _customerService.GetCustomerWithAddressById(id);

            if (customer == null)
                return NotFound("Customer not found");

            return Ok(customer);
        }


        [HttpGet("mealtypes")]
        public async Task<ActionResult<List<MealType>>> GetAllMealTypes()
        {
            var mealTypes = await _customerService.GetAllMealTypes();

            if (mealTypes == null || mealTypes.Count == 0)
            {
                return NotFound("No meal types found.");
            }

            return Ok(mealTypes);
        }

    }
}
