using HoneyShop.Models.DTOs;
using HoneyShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HoneyShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {

        private readonly IOrderService orderService;
        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                return Ok(await orderService.GetAllOrdersAsync());
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize(Roles = "User")]
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var userPhone = User.FindFirst("phone")?.Value;
            if (string.IsNullOrEmpty(userPhone))
            {
                return Unauthorized("Ошибка аутентификации");
            }
            var order = await orderService.GetOrderByIdAsync(orderId);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            if (order.phone != userPhone)
            {
                return Forbid("Вы не можете просматривать этот заказ");
            }

            return Ok(order);
        }
        [Authorize(Roles = "User")]
        [HttpGet("by-phone")]
        public async Task<IActionResult> GetOrdersByPhoneNumber()
        {
  
            try
            {
                var userPhone = User.FindFirst("phone")?.Value;
                if (string.IsNullOrEmpty(userPhone))
                {
                    return Unauthorized("Ошибка аутентификации");
                }
                return Ok(await orderService.GetOrderByPhoneNumberAsync(userPhone));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderDto order)
        {
            try
            {
                await orderService.AddOrderAsync(order);
                return Ok(new { success = true, message = "Order added successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}")]
        public async Task<IActionResult> PutOrder(int orderId, [FromBody] OrderDto order)
        {
            try
            {
                await orderService.UpdateOrderAsync(orderId, order);
                return Ok("Order updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            try
            {
                await orderService.DeleteOrderAsync(orderId);
                return Ok("Order deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
