using HotPotAPI.Interfaces;
using HotPotAPI.Models;
using HotPotAPI.Models.DTOs;
using HotPotAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotPotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryPartnerController : ControllerBase
    {
        private readonly IDeliveryPartnerService _deliveryPartnerService;

        private readonly IOrderService _orderService;

        public DeliveryPartnerController(IDeliveryPartnerService deliveryPartnerService, IOrderService orderService)
        {
            _deliveryPartnerService = deliveryPartnerService;
            _orderService = orderService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<CreateDeliveryPartnerResponse>> Register(CreateDeliveryPartnerRequest request)
        {
            try
            {
                var result = await _deliveryPartnerService.AddDeliveryPartner(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }


        // PUT: Update the delivery partner
        [HttpPut("update-partner/{id}")]
        public async Task<IActionResult> UpdateDeliveryPartner(int id, DeliveryPartnerUpdate updatedData)
        {
            var updated = await _deliveryPartnerService.UpdateDeliveryPartner(id, updatedData);
            if (updated == null)
                return NotFound($"Delivery partner with ID {id} not found");

            return Ok(updated);
        }

        // DELETE: delete delivery partner
        [HttpDelete("delete-partner/{id}")]
        public async Task<IActionResult> DeleteDeliveryPartner(int id)
        {
            try
            {
                var result = await _deliveryPartnerService.DeleteDeliveryPartner(id);
                if (!result)
                    return NotFound($"Delivery partner with ID {id} not found");

                return Ok($"Delivery partner with ID {id} deleted successfully");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // 409 Conflict
            }
        }

        [HttpGet("GetById/{partnerId}")]
        public async Task<ActionResult<DeliveryPartner>> GetDeliveryPartnerById(int partnerId)
        {
            try
            {
                var partner = await _deliveryPartnerService.GetDeliveryPartnerById(partnerId);
                return Ok(partner);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPut("delivery/update-status/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatusByDeliveryPartner(int orderId, [FromBody] int statusId)
        {
            try
            {
                var order = await _orderService.GetOrderById(orderId);
                if (order == null) return NotFound("Order not found");

                await _orderService.UpdateOrderStatusByDeliveryPartner(orderId, statusId);
                return Ok("Order status updated by delivery partner");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Getall")]
        public async Task<ActionResult<List<DeliveryPartner>>> GetAllDeliveryPartners()
        {
            try
            {
                var partners = await _deliveryPartnerService.GetAllDeliveryPartners();
                return Ok(partners);
            }
            catch (System.Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
