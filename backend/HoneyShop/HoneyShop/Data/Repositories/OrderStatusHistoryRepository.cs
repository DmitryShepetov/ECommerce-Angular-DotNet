using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HoneyShop.Data.Repositories
{
    public class OrderStatusHistoryRepository : IOrderStatusHistoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public OrderStatusHistoryRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<IEnumerable<OrderStatusHistory>> GetByIdOrderAsync(int orderId, CancellationToken cancellationToken = default) => 
            await _appDbContext.OrderStatusHistories.Where(o => o.OrderId == orderId).ToListAsync(cancellationToken);

        public async Task<OrderStatusHistory> GetByIdAsync(int id, CancellationToken cancellationToken = default) => 
            await _appDbContext.OrderStatusHistories.FindAsync(new object[] { id }, cancellationToken);
        public async Task AddAsync(OrderStatusHistory order, CancellationToken cancellationToken = default)
        {
            await _appDbContext.OrderStatusHistories.AddAsync(order, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdateAsync(OrderStatusHistory order, CancellationToken cancellationToken = default)
        {
            _appDbContext.OrderStatusHistories.Update(order);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await _appDbContext.OrderStatusHistories.FindAsync(new object[] { id }, cancellationToken);
            if (order != null)
            {
                _appDbContext.OrderStatusHistories.Remove(order);
                await _appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
