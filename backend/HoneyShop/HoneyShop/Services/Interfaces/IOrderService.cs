using HoneyShop.Models.DTOs;

namespace HoneyShop.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync();
        Task<OrderDto> GetOrderByIdAsync(int id);
        Task<IEnumerable<OrderDto>> GetOrderByPhoneNumberAsync(string phone);
        Task AddOrderAsync(OrderDto orderDto);
        Task DeleteOrderAsync(int id);
        Task UpdateOrderAsync(int id, OrderDto orderDto);
    }
}
