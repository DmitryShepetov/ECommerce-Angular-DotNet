using HoneyShop.Data.Entities;
using HoneyShop.Models.DTOs;
using System.Security.Claims;

namespace HoneyShop.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<IEnumerable<UserAllDto>> GetAllUsers();
        string GetUserFromToken(HttpContext httpContext);
        Task<UserUpdateDto> UpdateProfile(UserUpdateDto userUpdateDto, ClaimsPrincipal userPrincipal);
        Task<string> UploadUserImage(IFormFile file);
        Task<UserProfileDto> GetUserProfileAsync(ClaimsPrincipal user);
        Task<TokenRefreshResponseDto> RefreshTokenAsync(string refreshRequest);
        Task RevokeTokenAsync(string username);
        //Task<AuthResponseDto> LoginWithGoogleAsync(string googleToken);
        Task<ImageDto> GetImage(string fileName);
    }
}
