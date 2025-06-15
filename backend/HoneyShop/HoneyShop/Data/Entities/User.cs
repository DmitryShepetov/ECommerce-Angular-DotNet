using System.ComponentModel.DataAnnotations;

namespace HoneyShop.Data.Entities
{
    public class User
    {
        public int id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
        public string? passwordHash { get; set; }
        public string role { get; set; }
        public string image { get; set; }
        public string date { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
