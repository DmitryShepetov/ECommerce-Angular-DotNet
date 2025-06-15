namespace HoneyShop.Models.DTOs
{
    public class TokenRefreshRequestDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
