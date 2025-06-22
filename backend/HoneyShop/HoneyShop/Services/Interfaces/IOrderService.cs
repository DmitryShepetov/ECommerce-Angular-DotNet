using HoneyShop.Models.DTOs;

namespace HoneyShop.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDto>> GetAllOrdersAsync(CancellationToken cancellationToken = default);
        Task<OrderDto> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<OrderDto>> GetOrderByPhoneAsync(string phone, CancellationToken cancellationToken = default);
        Task AddOrderAsync(OrderDto orderDto, CancellationToken cancellationToken = default);
        Task DeleteOrderAsync(int id, CancellationToken cancellationToken = default);
        Task UpdateOrderAsync(int id, OrderDto orderDto, CancellationToken cancellationToken = default);
        Task<(List<OrderDto> Orders, int TotalCount)> GetPaginatedOrdersAsync(PaginationParameters paginationParams, CancellationToken cancellationToken);
    }
}
