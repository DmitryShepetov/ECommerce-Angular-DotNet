using HoneyShop.Data.Entities;

namespace HoneyShop.Data.Repositories.Interfaces
{
    public interface IOrderStatusHistoryRepository
    {
        Task<IEnumerable<OrderStatusHistory>> GetByIdOrderAsync(int idOrder);
        Task<OrderStatusHistory> GetByIdAsync(int id);
        Task AddAsync(OrderStatusHistory order);
        Task UpdateAsync(OrderStatusHistory order);
        Task DeleteAsync(int id);
    }
}
