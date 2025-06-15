using HoneyShop.Models.DTOs;

namespace HoneyShop.Services.Interfaces
{
    public interface IOrderStatusHistoryService
    {
        Task<IEnumerable<OrderStatusHistoryDto>> GetOrderHistoryStatusByIdAsync(int idOrder);
        Task AddOrderAsync(OrderStatusHistoryDto orderStatusDto);
        Task DeleteOrderAsync(int id);
        Task UpdateOrderAsync(int id, OrderStatusHistoryDto orderStatusHistoryDto);

    }
}
