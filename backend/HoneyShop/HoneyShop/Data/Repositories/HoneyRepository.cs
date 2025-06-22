using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HoneyShop.Data.Repositories
{
    public class HoneyRepository : IHoneyRepository
    {
        private readonly AppDbContext _appDbContext;
        public HoneyRepository(AppDbContext appDBContext)
        {
            _appDbContext = appDBContext;
        }
        public async Task<IEnumerable<Honey>> GetAllAsync(CancellationToken cancellationToken = default) => 
            await _appDbContext.Honeys.ToListAsync(cancellationToken);

        public async Task<IEnumerable<Honey>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default) => 
            await _appDbContext.Honeys.Include(x => x.Category).ToListAsync(cancellationToken);

        public async Task<Honey> GetByIdAsync(int honeyId, CancellationToken cancellationToken = default) => 
            await _appDbContext.Honeys.FindAsync(new object[] { honeyId }, cancellationToken);

        public async Task AddAsync(Honey honey, CancellationToken cancellationToken = default)
        {
            await _appDbContext.Honeys.AddAsync(honey, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdateAsync(Honey honey, CancellationToken cancellationToken = default)
        {
            _appDbContext.Honeys.Update(honey);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var honey = await _appDbContext.Honeys.FindAsync(new object[] { id }, cancellationToken);
            if (honey != null)
            {
                _appDbContext.Honeys.Remove(honey);
                await _appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
