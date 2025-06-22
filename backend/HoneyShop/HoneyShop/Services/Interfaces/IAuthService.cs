using HoneyShop.Data.Entities;
using HoneyShop.Models.DTOs;
using System.Security.Claims;

namespace HoneyShop.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
        Task<IEnumerable<UserAllDto>> GetAllUsers(CancellationToken cancellationToken = default);
        string GetUserFromToken(HttpContext httpContext);
        Task<UserUpdateDto> UpdateProfile(UserUpdateDto userUpdateDto, ClaimsPrincipal userPrincipal, CancellationToken cancellationToken = default);
        Task<string> UploadUserImage(IFormFile file, CancellationToken cancellationToken = default);
        Task<UserProfileDto> GetUserProfileAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default);
        Task<TokenRefreshResponseDto> RefreshTokenAsync(string refreshRequest, CancellationToken cancellationToken = default);
        Task RevokeTokenAsync(string username, CancellationToken cancellationToken = default);
        //Task<AuthResponseDto> LoginWithGoogleAsync(string googleToken);
        ImageDto GetImage(string fileName);
    }
}
