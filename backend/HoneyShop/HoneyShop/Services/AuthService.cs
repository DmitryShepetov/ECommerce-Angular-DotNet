using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using HoneyShop.Data.Entities;
using HoneyShop.Data.Repositories.Interfaces;
using HoneyShop.Models.DTOs;
using HoneyShop.Services.Interfaces;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;
using SixLabors.ImageSharp;


public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly TimeProvider _timeProvider;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public AuthService(
        IConfiguration configuration,
        IUserRepository userRepository,
        TimeProvider timeProvider)
    {
        _userRepository = userRepository;
        _configuration = configuration;

        _timeProvider = timeProvider;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
    {

        // Валидация входных данных
        if (registerDto == null)
            throw new ArgumentNullException(nameof(registerDto));

        if (string.IsNullOrWhiteSpace(registerDto.Phone) || !Regex.IsMatch(registerDto.Phone, @"^\+?\d{6,15}$"))
            throw new ArgumentException("Номер телефона должен быть от 6 до 15 символов.");

        if (string.IsNullOrWhiteSpace(registerDto.Username) || registerDto.Username.Length < 2 || registerDto.Username.Length > 30)
            throw new ArgumentException("Имя пользователя должно быть от 2 до 30 символов.");

        // Проверка имени пользователя
        var existingUser = await _userRepository.GetByUsernameAsync(registerDto.Username, cancellationToken);
        if (existingUser != null)
        {
            throw new ArgumentException("Username is already taken.");
        }

        // Проверка email
        var existingEmailUser = await _userRepository.GetByEmailAsync(registerDto.Email, cancellationToken);
        if (existingEmailUser != null)
        {
            throw new ArgumentException("Email is already taken.");
        }

        var existingPhoneUser = await _userRepository.GetByPhoneAsync(registerDto.Phone, cancellationToken);
        if (existingPhoneUser != null)
        {
            throw new ArgumentException("Phone is already taken.");
        }

        // Проверка сложности пароля
        var passwordValidationResult = ValidatePasswordStrength(registerDto.Password);
        if (!passwordValidationResult.IsValid)
        {
            throw new ArgumentException(passwordValidationResult.ErrorMessage);
        }

        // Хеширование пароля
        var passwordHash = HashPassword(registerDto.Password);

        var user = new User
        {
            Username = registerDto.Username,
            PasswordHash = passwordHash,
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            Phone = registerDto.Phone,
            Date = registerDto.Date,
            Role = "User", // По умолчанию
            Image = "/wwwroot/privateProfile/profileAvatar.png",
            RefreshToken = null,
            RefreshTokenExpiryTime = DateTime.MinValue
        };

        await _userRepository.AddUserAsync(user, cancellationToken);

        var tokens = await GenerateTokens(user, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = tokens.AccessToken
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        if (loginDto == null)
            throw new ArgumentNullException(nameof(loginDto));

        var user = await _userRepository.GetByUsernameAsync(loginDto.Username, cancellationToken);
        if (user == null)
        {
            // Для безопасности не сообщаем, что пользователь не найден
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        if (!VerifyPassword(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        // Генерация токенов
        var tokens = await GenerateTokens(user, cancellationToken);

        return new AuthResponseDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }

    public async Task<TokenRefreshResponseDto> RefreshTokenAsync(string refreshRequest, CancellationToken cancellationToken = default)
    {
        if (refreshRequest == null)
            throw new ArgumentNullException(nameof(refreshRequest));

        var user = await _userRepository.GetUserByTokenAsync(refreshRequest, cancellationToken);
        if (user == null || user.RefreshTokenExpiryTime <= _timeProvider.GetLocalNow())
            throw new SecurityTokenException("Invalid refresh token");

        // Генерация новых токенов
        var newTokens = await GenerateTokens(user, cancellationToken);

        return new TokenRefreshResponseDto
        {
            AccessToken = newTokens.AccessToken,
            RefreshToken = newTokens.RefreshToken
        };
    }

    public async Task RevokeTokenAsync(string username, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (user == null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;
        await _userRepository.UpdateUserAsync(user, cancellationToken);
    }

    public async Task<UserUpdateDto> UpdateProfile(UserUpdateDto userUpdateDto, ClaimsPrincipal userPrincipal, CancellationToken cancellationToken = default)
    {
        if (userUpdateDto == null)
            throw new ArgumentNullException(nameof(userUpdateDto));

        if (userPrincipal == null)
            throw new UnauthorizedAccessException("User not authenticated.");

        var currentUsername = userPrincipal.Identity?.Name;
        if (string.IsNullOrEmpty(currentUsername))
            throw new UnauthorizedAccessException("Invalid user identity.");

        // Получаем пользователя из базы данных
        var user = await _userRepository.GetByUsernameAsync(currentUsername, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        // Проверяем, что пользователь не пытается изменить чужие данные
        if (user.Username != userUpdateDto.Username)
        {
            throw new UnauthorizedAccessException("You cannot modify another user's data.");
        }

        // Обновляем данные пользователя
        user.FirstName = userUpdateDto.FirstName;
        user.LastName = userUpdateDto.LastName;
        user.Email = userUpdateDto.Email;
        user.Phone = userUpdateDto.Phone;
        user.Image = userUpdateDto.Image;
        user.Date = userUpdateDto.Date;

        // Обновляем пользователя в базе данных
        await _userRepository.UpdateUserAsync(user, cancellationToken);

        return new UserUpdateDto
        {
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Phone = user.Phone,
            Image = user.Image,
            Date = user.Date
        };
    }


    private async Task<(string AccessToken, string RefreshToken)> GenerateTokens(User user, CancellationToken cancellationToken = default)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("phone", user.Phone),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured")));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = _timeProvider.GetLocalNow().AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpiryMinutes"])).DateTime,
            SigningCredentials = creds,
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = _tokenHandler.CreateToken(tokenDescriptor);
        var accessToken = _tokenHandler.WriteToken(token);

        // Генерация refresh token
        var refreshToken = GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = _timeProvider.GetLocalNow().AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpiryDays"])).DateTime;

        await _userRepository.UpdateUserAsync(user, cancellationToken);


        return (accessToken, refreshToken);
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new ArgumentNullException(nameof(token));

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured"))),
            ValidateLifetime = false // Разрешаем expired token
        };

        try
        {
            var principal = _tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                jwtSecurityToken.Header.Alg != SecurityAlgorithms.HmacSha256)
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch (Exception ex)
        {
            throw new SecurityTokenException("Token validation failed", ex);
        }
    }

    private string HashPassword(string password)
    {
        // Используем PBKDF2 с HMAC-SHA256, 100,000 итераций
        byte[] salt = new byte[128 / 8];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 100000,
            numBytesRequested: 256 / 8));

        // Сохраняем соль и хеш вместе (формат: iterations.salt.hash)
        return $"{100000}.{Convert.ToBase64String(salt)}.{hashed}";
    }

    //public async Task<AuthResponseDto> LoginWithGoogleAsync(string googleToken)
    //{
    //    if (googleToken == null)
    //        throw new ArgumentNullException(nameof(googleToken));

    //    var payload = await ValidateGoogleTokenAsync(googleToken);
    //    if (payload == null)
    //    {
    //        throw new UnauthorizedAccessException("Invalid Google token.");
    //    }
    //    var existingUser = await _userRepository.GetByEmailAsync(payload.Email);
    //    if (existingUser != null)
    //    {
    //        // User exists - log them in
    //        return await GenerateAuthResponseForUser(existingUser);
    //    }
    //    var newUser = new User
    //    {
    //        username = GenerateUsernameFromEmail(payload.Email),
    //        email = payload.Email,
    //        firstName = payload.GivenName,
    //        lastName = payload.FamilyName,
    //        passwordHash = null, // No password for Google auth
    //        phone = "", // Can be updated later
    //        role = "User",
    //        image = "/assets/profileAvatar.png",
    //        date = DateTime.UtcNow.ToString("o"),
    //        RefreshToken = null,
    //        RefreshTokenExpiryTime = DateTime.MinValue
    //    };
    //    await _userRepository.AddUserAsync(newUser);

    //    return await GenerateAuthResponseForUser(newUser);

    //}

    //private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleTokenAsync(string token)
    //{
    //    try
    //    {
    //        var settings = new GoogleJsonWebSignature.ValidationSettings()
    //        {
    //            Audience = new[] { _configuration["Google:ClientId"] }
    //        };

    //        var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

    //        if (string.IsNullOrEmpty(payload.Email))
    //        {
    //            throw new SecurityTokenException("Google token does not contain email");
    //        }

    //        return payload;
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new SecurityTokenException("Google token validation failed", ex);
    //    }
    //}

    //private string GenerateUsernameFromEmail(string email)
    //{
    //    var username = email.Split('@')[0];
    //    username = Regex.Replace(username, @"[^a-zA-Z0-9]", "");

    //    // Check if username exists and append random numbers if needed
    //    var random = new Random();
    //    while (_userRepository.GetByUsernameAsync(username).Result != null)
    //    {
    //        username += random.Next(100, 999);
    //    }

    //    return username;
    //}

    //private async Task<AuthResponseDto> GenerateAuthResponseForUser(User user)
    //{
    //    var tokens = await GenerateTokens(user);

    //    return new AuthResponseDto
    //    {
    //        AccessToken = tokens.AccessToken,
    //        RefreshToken = tokens.RefreshToken
    //    };
    //}

    private bool VerifyPassword(string password, string storedHash)
    {
        var parts = storedHash.Split('.', 3);
        if (parts.Length != 3)
        {
            return false; 
        }

        var iterations = Convert.ToInt32(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var expectedHash = parts[2];

        var actualHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: iterations,
            numBytesRequested: 256 / 8));

        return actualHash == expectedHash;
    }

    public async Task<string> UploadUserImage(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Файл не выбран");
        }


        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new ArgumentException("Допустимы только файлы JPG и PNG");
        }


        using (var image = await Image.LoadAsync(file.OpenReadStream(), cancellationToken))
        {
            if (image.Width < 800 || image.Height < 800)
            {
                throw new ArgumentException("Изображение должно быть не менее 800x800 пикселей");
            }
        }

        // Создание папки uploads, если её нет
        string appDataFolder = Path.Combine(Directory.GetCurrentDirectory(), "AppData");
        string profileImagesFolder = Path.Combine(appDataFolder, "ProfileImages");
        if (!Directory.Exists(profileImagesFolder))
            Directory.CreateDirectory(profileImagesFolder);

        string fileName = Guid.NewGuid().ToString() + fileExtension;
        string filePath = Path.Combine(profileImagesFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }
        return fileName;
    }

    public ImageDto GetImage(string fileName)
    {
        //var user = await _userRepository.GetByUsernameAsync(userName);
        //if(user.image != fileName)
        //{
        //    throw new UnauthorizedAccessException("Вы не имеете доступа к этому изображению");
        //}

        string appDataFolder = Path.Combine(Directory.GetCurrentDirectory(), "AppData");
        string profileImagesFolder = Path.Combine(appDataFolder, "ProfileImages");
        string filePath = Path.Combine(profileImagesFolder, fileName);


        if (!System.IO.File.Exists(filePath))
            throw new ArgumentException("Изображение не найдено");


        string mimeType;
        switch (Path.GetExtension(fileName).ToLower())
        {
            case ".jpg":
            case ".jpeg":
                mimeType = "image/jpeg";
                break;
            case ".png":
                mimeType = "image/png";
                break;
            default:
                throw new ArgumentException("Неподдерживаемый формат изображения");
        }
        return new ImageDto
        {
            MimeType = mimeType,
            FilePath = filePath
        };
    }

    public string GetUserFromToken(HttpContext httpContext)
    {
        var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
        var principal = GetPrincipalFromExpiredToken(token);
        return principal.Identity?.Name;
    }

    public async Task<IEnumerable<UserAllDto>> GetAllUsers(CancellationToken cancellationToken = default)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        return users.Select(p => new UserAllDto
        {
            Username = p.Username,
            FirstName = p.FirstName,
            LastName = p.LastName,
            Email = p.Email,
            Phone = p.Phone,
            Role = p.Role
        });
    }

    public async Task<UserProfileDto> GetUserProfileAsync(ClaimsPrincipal user, CancellationToken cancellationToken = default)
    {
        var username = user.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            throw new UnauthorizedAccessException("User not authenticated.");

        var dbUser = await _userRepository.GetByUsernameAsync(username, cancellationToken);
        if (dbUser == null)
            throw new UnauthorizedAccessException("User not found.");

        return new UserProfileDto
        {
            Username = dbUser.Username,
            FirstName = dbUser.FirstName,
            LastName = dbUser.LastName,
            Email = dbUser.Email,
            Phone = dbUser.Phone,
            Image = dbUser.Image,
            Date = dbUser.Date
        };
    }
    private (bool IsValid, string ErrorMessage) ValidatePasswordStrength(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return (false, "Password cannot be empty.");

        var errors = new StringBuilder();

        if (password.Length < 12)
            errors.AppendLine("Minimum password length should be 12 characters.");

        if (!Regex.IsMatch(password, "[a-z]"))
            errors.AppendLine("Password should contain at least one lowercase letter.");

        if (!Regex.IsMatch(password, "[A-Z]"))
            errors.AppendLine("Password should contain at least one uppercase letter.");

        if (!Regex.IsMatch(password, "[0-9]"))
            errors.AppendLine("Password should contain at least one digit.");

        // Проверка на распространенные пароли
        var commonPasswords = new[] { "password", "123456", "qwerty" };
        if (commonPasswords.Contains(password.ToLower()))
            errors.AppendLine("Password is too common and insecure.");

        return errors.Length == 0
            ? (true, string.Empty)
            : (false, errors.ToString());
    }
}


