using HoneyShop.Data.Entities;

namespace HoneyShop.Data.Repositories.Interfaces
{
    public interface IHoneyRepository
    {
        Task<IEnumerable<Honey>> GetAllAsync();
        Task<Honey> GetByIdAsync(int honeyId);
        Task AddAsync(Honey honey);
        Task UpdateAsync(Honey honey);
        Task<IEnumerable<Honey>> GetAllWithCategoryAsync();
        Task DeleteAsync(int id);
    }
}
