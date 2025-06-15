using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace HoneyShop.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDBContext appDBContext;
        public CategoryRepository(AppDBContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }
        public async Task<IEnumerable<Category>> GetAllAsync() => await appDBContext.Category.ToListAsync();
        public async Task<Category> GetByIdAsync(int categoryId) => await appDBContext.Category.FindAsync(categoryId);
        public async Task AddAsync(Category category)
        {
            await appDBContext.Category.AddAsync(category);
            await appDBContext.SaveChangesAsync();
        }
        public async Task UpdateAsync(Category category)
        {
            appDBContext.Category.Update(category);
            await appDBContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var category = await appDBContext.Category.FindAsync(id);
            if (category != null)
            {
                appDBContext.Category.Remove(category);
                await appDBContext.SaveChangesAsync();
            }
        }
    }
}
