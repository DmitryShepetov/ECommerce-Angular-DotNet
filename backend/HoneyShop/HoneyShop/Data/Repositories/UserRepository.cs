using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HoneyShop.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _appDbContext;
        public UserRepository(AppDbContext appDBContext)
        {
            _appDbContext = appDBContext;
        }
   

        // Получение пользователя по ID
        public async Task<User> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Users.FindAsync(new object[] { id }, cancellationToken);
        }

        // Получение пользователя по Username
        public async Task<User> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<User> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }


        public async Task<User> GetUserByTokenAsync(string token, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == token, cancellationToken);
        }

        public async Task<User> GetByPhoneAsync(string phone, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Users.FirstOrDefaultAsync(u => u.Phone == phone, cancellationToken);
        }
        // Получение всех пользователей
        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Users.ToListAsync(cancellationToken);
        }

        // Добавление нового пользователя
        public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
        {
            await _appDbContext.Users.AddAsync(user, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }

        // Обновление информации о пользователе
        public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            _appDbContext.Users.Update(user);
            await _appDbContext.SaveChangesAsync(cancellationToken);
        }

        // Удаление пользователя по ID
        public async Task DeleteUserAsync(int id, CancellationToken cancellationToken = default)
        {
            var user = await _appDbContext.Users.FindAsync(new object[] { id }, cancellationToken);
            if (user != null)
            {
                _appDbContext.Users.Remove(user);
                await _appDbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
