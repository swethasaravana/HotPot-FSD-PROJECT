using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace HotPotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // 1. Place Order
        [HttpPost("place-order")]
        public async Task<IActionResult> PlaceOrder([FromBody] CreateOrderRequest request)
        {
            try
            {
                if (request == null) return BadRequest("Invalid order data");

                var order = await _orderService.PlaceOrder(request.CustomerId, request.CustomerAddressId,
                                                            request.PaymentMethodId, request.PaymentStatusId);

                if (order == null) return BadRequest("Unable to place order");

                return Ok(order);
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // 2. Get Order by ID
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                if (order == null) return NotFound("Order not found");

                return Ok(order);
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // 3. Get Orders by Customer ID
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetOrdersByCustomer(int customerId)
        {
            try
            {
                List<OrderResponseDTO> orders = await _orderService.GetOrdersByCustomer(customerId);
                if (orders == null || !orders.Any())
                {
                    return NotFound("No orders found for this customer.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // 4. Get Orders by Status
        [HttpGet("status/{statusId}")]
        public async Task<IActionResult> GetOrdersByStatus(int statusId)
        {
            try
            {
                var orders = await _orderService.GetOrdersByStatus(statusId);
                if (orders == null || !orders.Any())
                {
                    return NotFound("No orders found with the specified status.");
                }

                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // 5. Update Order Status
        [HttpPut("update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromBody] int statusId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                if (order == null)
                {
                    return NotFound("Order not found");
                }

                await _orderService.UpdateOrderStatus(orderId, statusId);
                return Ok("Order status updated successfully");
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // 6. Get All Orders (Admin)
        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                // Optionally log the exception here
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("delivery-partner/{deliveryPartnerId}/orders")]
        public async Task<IActionResult> GetDeliveryPartnerOrders(int deliveryPartnerId)
        {
            try
            {
                var orders = await _orderService.GetDeliveryPartnerOrders(deliveryPartnerId);
                if (orders == null || !orders.Any())
                    return NotFound("No orders found for this delivery partner");

                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}