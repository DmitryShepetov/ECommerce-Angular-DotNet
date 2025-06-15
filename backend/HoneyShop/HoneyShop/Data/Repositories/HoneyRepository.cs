using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HoneyShop.Data.Repositories
{
    public class HoneyRepository : IHoneyRepository
    {
        private readonly AppDBContext appDBContext;
        public HoneyRepository(AppDBContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }
        public async Task<IEnumerable<Honey>> GetAllAsync() => await appDBContext.Honey.ToListAsync();
        public async Task<IEnumerable<Honey>> GetAllWithCategoryAsync() => await appDBContext.Honey.Include(x => x.Category).ToListAsync();
        public async Task<Honey> GetByIdAsync(int honeyId) => await appDBContext.Honey.FindAsync(honeyId);
        public async Task AddAsync(Honey honey)
        {
            await appDBContext.Honey.AddAsync(honey);
            await appDBContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(Honey honey)
        {
            appDBContext.Honey.Update(honey);
            await appDBContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var honey = await appDBContext.Honey.FindAsync(id);
            if (honey != null)
            {
                appDBContext.Honey.Remove(honey);
                await appDBContext.SaveChangesAsync();
            }
        }
    }
}
