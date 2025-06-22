using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace HoneyShop.Data.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _appDbContext;
        public CategoryRepository(AppDbContext appDBContext)
        {
            _appDbContext = appDBContext;
        }
        public async Task<IEnumerable<Category>> GetAllAsync(CancellationToken cancellationToken = default) => 
            await _appDbContext.Categories.ToListAsync(cancellationToken);

        public async Task<Category> GetByIdAsync(int categoryId, CancellationToken cancellationToken = default) => 
            await _appDbContext.Categories.FindAsync(new object[] { categoryId }, cancellationToken);

        public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            await _appDbContext.Categories.AddAsync(category, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
        {
            _appDbContext.Categories.Update(category);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var category = await _appDbContext.Categories.FindAsync(new object[] { id }, cancellationToken);
            if (category != null)
            {
                _appDbContext.Categories.Remove(category);
                await _appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
