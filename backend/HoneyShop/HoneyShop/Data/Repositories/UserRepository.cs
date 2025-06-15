using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HoneyShop.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDBContext appDBContext;
        public UserRepository(AppDBContext appDBContext)
        {
            this.appDBContext = appDBContext;
        }
   

        // Получение пользователя по ID
        public async Task<User> GetByIdAsync(int id)
        {
            return await appDBContext.Users.FindAsync(id);
        }

        // Получение пользователя по Username
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await appDBContext.Users.FirstOrDefaultAsync(u => u.username == username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await appDBContext.Users.FirstOrDefaultAsync(u => u.email == email);
        }


        public async Task<User> GetUserByTokenAsync(string token)
        {
            return await appDBContext.Users.FirstOrDefaultAsync(u => u.RefreshToken == token);
        }

        public async Task<User> GetByPhoneAsync(string phone)
        {
            return await appDBContext.Users.FirstOrDefaultAsync(u => u.phone == phone);
        }
        // Получение всех пользователей
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await appDBContext.Users.ToListAsync();
        }

        // Добавление нового пользователя
        public async Task AddUserAsync(User user)
        {
            await appDBContext.Users.AddAsync(user);
            await appDBContext.SaveChangesAsync();
        }

        // Обновление информации о пользователе
        public async Task UpdateUserAsync(User user)
        {
            appDBContext.Users.Update(user);
            await appDBContext.SaveChangesAsync();
        }

        // Удаление пользователя по ID
        public async Task DeleteUserAsync(int id)
        {
            var user = await appDBContext.Users.FindAsync(id);
            if (user != null)
            {
                appDBContext.Users.Remove(user);
                await appDBContext.SaveChangesAsync();
            }
        }
    }
}
