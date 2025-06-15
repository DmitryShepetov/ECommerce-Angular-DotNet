using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace HoneyShop.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDBContext appDBContext;
        public OrderRepository(AppDBContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }
        public async Task<IEnumerable<Order>> GetAllAsync() => await appDBContext.Order.Include(o => o.Items).ToListAsync();
        public async Task<Order> GetByIdAsync(int idOrder) => await appDBContext.Order.Include(o => o.Items).FirstOrDefaultAsync(p => p.id == idOrder);
        public async Task<IEnumerable<Order>> GetOrderByPhoneNumberAsync(string phone) => await appDBContext.Order.Where(p => p.phone == phone).Include(o => o.Items).ToListAsync();
        public async Task AddAsync(Order order)
        {
            await appDBContext.Order.AddAsync(order);
            await appDBContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(Order order)
        {
            appDBContext.Order.Update(order);
            await appDBContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var order = await appDBContext.Order.FindAsync(id);
            if (order != null)
            {
                appDBContext.Order.Remove(order);
                await appDBContext.SaveChangesAsync();
            }
        }
    }
}
