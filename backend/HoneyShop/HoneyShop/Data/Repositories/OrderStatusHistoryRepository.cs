using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HoneyShop.Data.Repositories
{
    public class OrderStatusHistoryRepository : IOrderStatusHistoryRepository
    {
        private readonly AppDBContext appDBContext;
        public OrderStatusHistoryRepository(AppDBContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }
        public async Task<IEnumerable<OrderStatusHistory>> GetByIdOrderAsync(int idOrder) => await appDBContext.OrderStatusHistories.Where(o => o.OrderId == idOrder).ToListAsync();
        public async Task<OrderStatusHistory> GetByIdAsync(int id) => await appDBContext.OrderStatusHistories.FindAsync(id);
        public async Task AddAsync(OrderStatusHistory order)
        {
            await appDBContext.OrderStatusHistories.AddAsync(order);
            await appDBContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(OrderStatusHistory order)
        {
            appDBContext.OrderStatusHistories.Update(order);
            await appDBContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var order = await appDBContext.OrderStatusHistories.FindAsync(id);
            if (order != null)
            {
                appDBContext.OrderStatusHistories.Remove(order);
                await appDBContext.SaveChangesAsync();
            }
        }
    }
}
