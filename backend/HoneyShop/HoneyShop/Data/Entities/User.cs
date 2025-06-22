using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string? PasswordHash { get; set; }
        public string Role { get; set; }
        public string Image { get; set; }
        public string Date { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
