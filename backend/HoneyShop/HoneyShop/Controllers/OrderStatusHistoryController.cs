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
        private readonly IOrderStatusHistoryService _orderStatusHistoryService;
        private readonly IOrderService _orderService;
        public OrderStatusHistoryController(IOrderStatusHistoryService orderStatusHistoryService, IOrderService orderService)
        {
            _orderService = orderService;
            _orderStatusHistoryService = orderStatusHistoryService;
        }
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderHistoryStatusById(int orderId, CancellationToken cancellationToken)
        {
            try
            {
                var userPhone = User.FindFirst("phone")?.Value;
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);
                if (order.Phone != userPhone && userRole != "Admin")
                {
                    return Forbid("Вы не можете просматривать этот заказ");
                }
                return Ok(await _orderStatusHistoryService.GetOrderHistoryStatusByIdAsync(orderId, cancellationToken));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddOrderHistoryStatus([FromBody] OrderStatusHistoryDto orderStatusHistory, CancellationToken cancellationToken)
        {
            try
            {
                await _orderStatusHistoryService.AddOrderAsync(orderStatusHistory, cancellationToken);
                return Ok(new { success = true, message = "Order added successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> PutOrder(int orderId, [FromBody] OrderStatusHistoryDto orderStatusHistory, CancellationToken cancellationToken)
        {
            try
            {
                await _orderStatusHistoryService.UpdateOrderAsync(orderId, orderStatusHistory, cancellationToken);
                return Ok("Order updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderStatusHistoryId, CancellationToken cancellationToken)
        {
            try
            {
                await _orderStatusHistoryService.DeleteOrderAsync(orderStatusHistoryId, cancellationToken);
                return Ok("Order deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
