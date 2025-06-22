using HoneyShop.Data.Entities;

namespace HoneyShop.Data.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
        Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);
        Task AddUserAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
        Task DeleteUserAsync(int id, CancellationToken cancellationToken = default);
        Task<User> GetUserByTokenAsync(string token, CancellationToken cancellationToken = default);
        Task<User> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default);

    }
}
