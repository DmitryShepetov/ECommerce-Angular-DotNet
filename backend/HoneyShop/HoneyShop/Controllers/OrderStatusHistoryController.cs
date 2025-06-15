using HoneyShop.Data.Entities;
using HoneyShop.Models.DTOs;
using HoneyShop.Services;
using HoneyShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe.Climate;
using System.Security.Claims;

namespace HoneyShop.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrderStatusHistoryController : Controller
    {
        private readonly IOrderStatusHistoryService orderStatusHistoryService;
        private readonly IOrderService orderService;
        public OrderStatusHistoryController(IOrderStatusHistoryService orderStatusHistoryService, IOrderService orderService)
        {
            this.orderService = orderService;
            this.orderStatusHistoryService = orderStatusHistoryService;
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderHistoryStatusById(int orderId)
        {
            try
            {
                var userPhone = User.FindFirst("phone")?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var order = await orderService.GetOrderByIdAsync(orderId);
                if (order.phone != userPhone && userRole != "Admin")
                {
                    return Forbid("Вы не можете просматривать этот заказ");
                }
                return Ok(await orderStatusHistoryService.GetOrderHistoryStatusByIdAsync(orderId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddOrderHistoryStatus([FromBody] OrderStatusHistoryDto orderStatusHistory)
        {
            try
            {
                await orderStatusHistoryService.AddOrderAsync(orderStatusHistory);
                return Ok(new { success = true, message = "Order added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> PutOrder(int orderId, [FromBody] OrderStatusHistoryDto orderStatusHistory)
        {
            try
            {
                await orderStatusHistoryService.UpdateOrderAsync(orderId, orderStatusHistory);
                return Ok("Order updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderStatusHistoryId)
        {
            try
            {
                await orderStatusHistoryService.DeleteOrderAsync(orderStatusHistoryId);
                return Ok("Order deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
