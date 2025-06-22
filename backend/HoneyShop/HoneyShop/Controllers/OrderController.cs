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

        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllOrders(CancellationToken cancellationToken)
        {
            try
            {
                return Ok(await _orderService.GetAllOrdersAsync(cancellationToken));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }
        [Authorize(Roles = "User")]
        [HttpGet("{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId, CancellationToken cancellationToken)
        {
            var userPhone = User.FindFirst("phone")?.Value;
            if (string.IsNullOrEmpty(userPhone))
            {
                return Unauthorized("Ошибка аутентификации");
            }
            var order = await _orderService.GetOrderByIdAsync(orderId, cancellationToken);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            if (order.Phone != userPhone)
            {
                return Forbid("Вы не можете просматривать этот заказ");
            }

            return Ok(order);
        }
        [Authorize(Roles = "User")]
        [HttpGet("by-phone")]
        public async Task<IActionResult> GetOrdersByPhoneNumber(CancellationToken cancellationToken)
        {
  
            try
            {
                var userPhone = User.FindFirst("phone")?.Value;
                if (string.IsNullOrEmpty(userPhone))
                {
                    return Unauthorized("Ошибка аутентификации");
                }
                return Ok(await _orderService.GetOrderByPhoneAsync(userPhone, cancellationToken));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddOrder([FromBody] OrderDto order, CancellationToken cancellationToken)
        {
            try
            {
                await _orderService.AddOrderAsync(order, cancellationToken);
                return Ok(new { success = true, message = "Order added successfully." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{orderId}")]
        public async Task<IActionResult> PutOrder(int orderId, [FromBody] OrderDto order, CancellationToken cancellationToken)
        {
            try
            {
                await _orderService.UpdateOrderAsync(orderId, order, cancellationToken);
                return Ok("Order updated successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpGet("paged")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPagedOrders([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var paginationParams = new PaginationParameters { PageNumber = page, PageSize = pageSize };
            var (orders, totalCount) = await _orderService.GetPaginatedOrdersAsync(paginationParams, cancellationToken);

            return Ok(new
            {
                Data = orders,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{orderId}")]
        public async Task<IActionResult> DeleteOrder(int orderId, CancellationToken cancellationToken)
        {
            try
            {
                await _orderService.DeleteOrderAsync(orderId, cancellationToken);
                return Ok("Order deleted successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
        }
    }
}
