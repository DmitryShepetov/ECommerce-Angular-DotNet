﻿using Azure;
using Azure.Core;
using HoneyShop.Data.Entities;
using HoneyShop.Models.DTOs;
using HoneyShop.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;




namespace HoneyShop.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Регистрация нового пользователя
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.RegisterAsync(registerDto, cancellationToken);
                SetRefreshTokenCookie(response.RefreshToken);
                return Ok(new { response.AccessToken });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Аутентификация пользователя
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _authService.LoginAsync(loginDto, cancellationToken);
                SetRefreshTokenCookie(response.RefreshToken);
                return Ok(new { response.AccessToken });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Обновление Access Token с помощью Refresh Token
        /// </summary>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(CancellationToken cancellationToken)
        {
            try
            {
                // Получаем refresh token из cookie
                var refreshToken = Request.Cookies["refreshToken"];
                if (string.IsNullOrEmpty(refreshToken))
                {
                    return Unauthorized(new { message = "Refresh token is missing" });
                }

                var response = await _authService.RefreshTokenAsync(refreshToken, cancellationToken);

                SetRefreshTokenCookie(response.RefreshToken);
                return Ok(new { response.AccessToken });
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Выход из системы (отзыв токена)
        /// </summary>
        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(CancellationToken cancellationToken)
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                    return Unauthorized();

                await _authService.RevokeTokenAsync(username, cancellationToken);

                Response.Cookies.Delete("refreshToken");

                return Ok(new { message = "Token revoked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [Authorize]
        [HttpPost("log-out")]
        public IActionResult LogOut()
        {
            try
            {
                var username = User.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                    return Unauthorized();


                Response.Cookies.Delete("refreshToken");

                return Ok(new { message = "Token revoked successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        /// <summary>
        /// Обновление профиля пользователя
        /// </summary>
        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserUpdateDto userUpdateDto, CancellationToken cancellationToken)
        {
            try
            {
                var updatedUser = await _authService.UpdateProfile(userUpdateDto, User, cancellationToken);
                return Ok(updatedUser);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        //[HttpPost("google-auth")]
        //public async Task<IActionResult> LoginWithGoogle([FromBody] string googleAuth)
        //{
        //    try
        //    {
        //        var response = await _authService.LoginWithGoogleAsync(googleAuth);
        //        SetRefreshTokenCookie(response.RefreshToken);
        //        return Ok(new { response.AccessToken });
        //    }
        //    catch (UnauthorizedAccessException ex)
        //    {
        //        return Unauthorized(new { message = ex.Message });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { message = "Internal server error", details = ex.Message });
        //    }
        //}

        /// <summary>
        /// Загрузка изображения профиля
        /// </summary>
        [Authorize]
        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile file, CancellationToken cancellationToken)
        {
            try
            {
                var imageUrl = await _authService.UploadUserImage(file, cancellationToken);
                return Ok(new { imageUrl });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "User not authenticated" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error uploading image", details = ex.Message });
            }
        }
        // [Authorize] Добавил [AllowAnonymous], нет времени добавлять токен к HttpClient запросу, костыль.
        [AllowAnonymous]
       
        [HttpGet("profile-image/{fileName}")]
        public IActionResult GetProfileImage(string fileName)
        {
            try
            {
                //var username = User.Identity?.Name;
                //if (string.IsNullOrEmpty(username))
                //    return Unauthorized();

               var image =  _authService.GetImage(fileName); // Я передавал username



                // Возвращаем файл как поток
                return PhysicalFile(image.FilePath, image.MimeType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ошибка при загрузке изображения", details = ex.Message });
            }
        }

        /// <summary>
        /// Получение списка всех пользователей (только для админов)
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
        {
            try
            {
                var users = await _authService.GetAllUsers(cancellationToken);
                return Ok(users);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Access denied" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

        [HttpGet("profile")]
        [Authorize]
        public async Task<IActionResult> GetProfile(CancellationToken cancellationToken)
        {
            try
            {
                var profile = await _authService.GetUserProfileAsync(User, cancellationToken);
                return Ok(profile);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}