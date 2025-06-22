using HoneyShop.Models.DTOs;

namespace HoneyShop.Services.Interfaces
{
    public interface IOrderStatusHistoryService
    {
        Task<IEnumerable<OrderStatusHistoryDto>> GetOrderHistoryStatusByIdAsync(int idOrder, CancellationToken cancellationToken = default);
        Task AddOrderAsync(OrderStatusHistoryDto orderStatusDto, CancellationToken cancellationToken = default);
        Task DeleteOrderAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateOrderAsync(int id, OrderStatusHistoryDto orderStatusHistoryDto, CancellationToken cancellationToken = default);

    }
}
