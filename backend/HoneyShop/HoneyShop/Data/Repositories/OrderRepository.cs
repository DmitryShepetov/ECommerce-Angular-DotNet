using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using HoneyShop.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

namespace HoneyShop.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _appDbContext;
        public OrderRepository(AppDbContext appDBContext)
        {
            _appDbContext = appDBContext;
        }
        public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default) => 
            await _appDbContext.Orders.Include(o => o.Items).ToListAsync(cancellationToken);

        public async Task<Order> GetOrderByIdAsync(int idOrder, CancellationToken cancellationToken = default) => 
            await _appDbContext.Orders.Include(o => o.Items).FirstOrDefaultAsync(p => p.Id == idOrder, cancellationToken);

        public async Task<IEnumerable<Order>> GetOrderByPhoneAsync(string phone, CancellationToken cancellationToken = default) => 
            await _appDbContext.Orders.Where(p => p.Phone == phone).Include(o => o.Items).ToListAsync(cancellationToken);

        public async Task<List<Order>> GetPaginatedOrdersAsync(PaginationParameters paginationParams, CancellationToken cancellationToken)
        {
            return await _appDbContext.Orders
                .Include(o => o.Items)
                .Include(o => o.StatusHistory)
                .OrderByDescending(o => o.DateTime)
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> GetTotalOrdersCountAsync(CancellationToken cancellationToken)
        {
            return await _appDbContext.Orders.CountAsync(cancellationToken);
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _appDbContext.Orders.AddAsync(order, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            _appDbContext.Orders.Update(order);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var order = await _appDbContext.Orders.FindAsync(new object[] { id }, cancellationToken);
            if (order != null)
            {
                _appDbContext.Orders.Remove(order);
                await _appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
