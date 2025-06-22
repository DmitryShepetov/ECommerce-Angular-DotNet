using HoneyShop.Data.Entities;

namespace HoneyShop.Data.Repositories.Interfaces
{
    public interface IOrderStatusHistoryRepository
    {
        Task<IEnumerable<OrderStatusHistory>> GetByIdOrderAsync(int orderId, CancellationToken cancellationToken = default);
        Task<OrderStatusHistory> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(OrderStatusHistory order, CancellationToken cancellationToken = default);
        Task UpdateAsync(OrderStatusHistory order, CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
