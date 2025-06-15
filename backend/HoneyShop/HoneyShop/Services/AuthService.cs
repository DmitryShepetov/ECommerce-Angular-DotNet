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

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {

        // Валидация входных данных
        if (registerDto == null)
            throw new ArgumentNullException(nameof(registerDto));

        if (string.IsNullOrWhiteSpace(registerDto.phone) || !Regex.IsMatch(registerDto.phone, @"^\+?\d{6,15}$"))
            throw new ArgumentException("Номер телефона должен быть от 6 до 15 символов.");

        if (string.IsNullOrWhiteSpace(registerDto.username) || registerDto.username.Length < 2 || registerDto.username.Length > 30)
            throw new ArgumentException("Имя пользователя должно быть от 2 до 30 символов.");

        // Проверка имени пользователя
        var existingUser = await _userRepository.GetByUsernameAsync(registerDto.username);
        if (existingUser != null)
        {
            throw new ArgumentException("Username is already taken.");
        }

        // Проверка email
        var existingEmailUser = await _userRepository.GetByEmailAsync(registerDto.email);
        if (existingEmailUser != null)
        {
            throw new ArgumentException("Email is already taken.");
        }

        var existingPhoneUser = await _userRepository.GetByPhoneAsync(registerDto.phone);
        if (existingPhoneUser != null)
        {
            throw new ArgumentException("Phone is already taken.");
        }

        // Проверка сложности пароля
        var passwordValidationResult = ValidatePasswordStrength(registerDto.password);
        if (!passwordValidationResult.IsValid)
        {
            throw new ArgumentException(passwordValidationResult.ErrorMessage);
        }

        // Хеширование пароля
        var passwordHash = HashPassword(registerDto.password);

        var user = new User
        {
            username = registerDto.username,
            passwordHash = passwordHash,
            firstName = registerDto.firstName,
            lastName = registerDto.lastName,
            email = registerDto.email,
            phone = registerDto.phone,
            date = registerDto.date,
            role = "User", // По умолчанию
            image = "/wwwroot/privateProfile/profileAvatar.png",
            RefreshToken = null,
            RefreshTokenExpiryTime = DateTime.MinValue
        };

        await _userRepository.AddUserAsync(user);

        var tokens = await GenerateTokens(user);

        return new AuthResponseDto
        {
            AccessToken = tokens.AccessToken
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        if (loginDto == null)
            throw new ArgumentNullException(nameof(loginDto));

        var user = await _userRepository.GetByUsernameAsync(loginDto.username);
        if (user == null)
        {
            // Для безопасности не сообщаем, что пользователь не найден
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        if (!VerifyPassword(loginDto.password, user.passwordHash))
        {
            throw new UnauthorizedAccessException("Invalid username or password.");
        }

        // Генерация токенов
        var tokens = await GenerateTokens(user);

        return new AuthResponseDto
        {
            AccessToken = tokens.AccessToken,
            RefreshToken = tokens.RefreshToken
        };
    }

    public async Task<TokenRefreshResponseDto> RefreshTokenAsync(string refreshRequest)
    {
        if (refreshRequest == null)
            throw new ArgumentNullException(nameof(refreshRequest));

        var user = await _userRepository.GetUserByTokenAsync(refreshRequest);
        if (user == null || user.RefreshTokenExpiryTime <= _timeProvider.GetLocalNow())
            throw new SecurityTokenException("Invalid refresh token");

        // Генерация новых токенов
        var newTokens = await GenerateTokens(user);

        return new TokenRefreshResponseDto
        {
            AccessToken = newTokens.AccessToken,
            RefreshToken = newTokens.RefreshToken
        };
    }

    public async Task RevokeTokenAsync(string username)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        if (user == null) return;

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.MinValue;
        await _userRepository.UpdateUserAsync(user);
    }

    public async Task<UserUpdateDto> UpdateProfile(UserUpdateDto userUpdateDto, ClaimsPrincipal userPrincipal)
    {
        if (userUpdateDto == null)
            throw new ArgumentNullException(nameof(userUpdateDto));

        if (userPrincipal == null)
            throw new UnauthorizedAccessException("User not authenticated.");

        var currentUsername = userPrincipal.Identity?.Name;
        if (string.IsNullOrEmpty(currentUsername))
            throw new UnauthorizedAccessException("Invalid user identity.");

        // Получаем пользователя из базы данных
        var user = await _userRepository.GetByUsernameAsync(currentUsername);
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found.");
        }

        // Проверяем, что пользователь не пытается изменить чужие данные
        if (user.username != userUpdateDto.username)
        {
            throw new UnauthorizedAccessException("You cannot modify another user's data.");
        }

        // Обновляем данные пользователя
        user.firstName = userUpdateDto.firstName;
        user.lastName = userUpdateDto.lastName;
        user.email = userUpdateDto.email;
        user.phone = userUpdateDto.phone;
        user.image = userUpdateDto.image;
        user.date = userUpdateDto.date;

        // Обновляем пользователя в базе данных
        await _userRepository.UpdateUserAsync(user);

        return new UserUpdateDto
        {
            username = user.username,
            firstName = user.firstName,
            lastName = user.lastName,
            email = user.email,
            phone = user.phone,
            image = user.image,
            date = user.date
        };
    }


    private async Task<(string AccessToken, string RefreshToken)> GenerateTokens(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.username),
            new Claim(ClaimTypes.Name, user.username),
            new Claim("phone", user.phone),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.role)
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

        await _userRepository.UpdateUserAsync(user);


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
            return false; // Неверный формат
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

    public async Task<string> UploadUserImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("Файл не выбран");
        }

        // Проверка допустимых форматов
        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension))
        {
            throw new ArgumentException("Допустимы только файлы JPG и PNG");
        }

        // Проверка размеров изображения
        using (var image = await Image.LoadAsync(file.OpenReadStream()))
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
            await file.CopyToAsync(stream);
        }
        return fileName;
    }

    public async Task<ImageDto> GetImage(string fileName) //Передавал username
    {
        //var user = await _userRepository.GetByUsernameAsync(userName);
        //if(user.image != fileName)
        //{
        //    throw new UnauthorizedAccessException("Вы не имеете доступа к этому изображению");
        //}
        // Путь к файлу
        string appDataFolder = Path.Combine(Directory.GetCurrentDirectory(), "AppData");
        string profileImagesFolder = Path.Combine(appDataFolder, "ProfileImages");
        string filePath = Path.Combine(profileImagesFolder, fileName);

        // Проверка существования файла
        if (!System.IO.File.Exists(filePath))
            throw new ArgumentException("Изображение не найдено");

        // Определение MIME-типа
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
            mimeType = mimeType,
            filePath = filePath
        };
    }

    public string GetUserFromToken(HttpContext httpContext)
    {
        var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", string.Empty);
        var principal = GetPrincipalFromExpiredToken(token);
        return principal.Identity?.Name;
    }

    public async Task<IEnumerable<UserAllDto>> GetAllUsers()
    {
        var category = await _userRepository.GetAllAsync();
        return category.Select(p => new UserAllDto
        {
            username = p.username,
            firstName = p.firstName,
            lastName = p.lastName,
            email = p.email,
            phone = p.phone,
            role = p.role
        });
    }

    public async Task<UserProfileDto> GetUserProfileAsync(ClaimsPrincipal user)
    {
        var username = user.Identity?.Name;
        if (string.IsNullOrEmpty(username))
            throw new UnauthorizedAccessException("User not authenticated.");

        var dbUser = await _userRepository.GetByUsernameAsync(username);
        if (dbUser == null)
            throw new UnauthorizedAccessException("User not found.");

        return new UserProfileDto
        {
            username = dbUser.username,
            firstName = dbUser.firstName,
            lastName = dbUser.lastName,
            email = dbUser.email,
            phone = dbUser.phone,
            image = dbUser.image,
            date = dbUser.date
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


