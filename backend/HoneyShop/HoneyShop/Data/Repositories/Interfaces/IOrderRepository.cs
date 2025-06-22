using HoneyShop.Data.Entities;
using HoneyShop.Models.DTOs;

namespace HoneyShop.Data.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Order> GetOrderByIdAsync(int orderId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetOrderByPhoneAsync(string phone, CancellationToken cancellationToken = default);
        Task AddAsync(Order order, CancellationToken cancellationToken = default);
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
        Task DeleteAsync(int orderId, CancellationToken cancellationToken = default);
        Task<List<Order>> GetPaginatedOrdersAsync(PaginationParameters paginationParams, CancellationToken cancellationToken);
        Task<int> GetTotalOrdersCountAsync(CancellationToken cancellationToken);
    }
}
