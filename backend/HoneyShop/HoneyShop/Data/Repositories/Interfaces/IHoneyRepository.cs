using HoneyShop.Data.Entities;

namespace HoneyShop.Data.Repositories.Interfaces
{
    public interface IHoneyRepository
    {
        Task<IEnumerable<Honey>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<Honey> GetByIdAsync(int honeyId, CancellationToken cancellationToken = default);
        Task AddAsync(Honey honey, CancellationToken cancellationToken = default);
        Task UpdateAsync(Honey honey, CancellationToken cancellationToken = default);
        Task<IEnumerable<Honey>> GetAllWithCategoryAsync(CancellationToken cancellationToken = default);
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    }
}
