using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GatewayService.Database.Tables;
using GatewayService.Interfaces.Repositories;
using GatewayService.Interfaces.Services;
using GatewayService.Models;
using GatewayService.Models.Dtos;
using Microsoft.IdentityModel.Tokens;
using GatewayService.Interfaces.Config;
using OtpNet;
using QRCoder;

namespace GatewayService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITwoFactorRepository _twoFactorRepository;
    private readonly IPasswordResetCodeService _passwordResetCodeService;
    private readonly IJwtConfiguration _jwtConfig;
    private readonly IEmailService _emailService;

    public UserService(
            IUserRepository userRepository,
            IRefreshTokenRepository refreshTokenRepository,
            ITwoFactorRepository twoFactorRepository,
            IPasswordResetCodeService passwordResetCodeService,
            IJwtConfiguration jwtConfig,
            IEmailService emailService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _twoFactorRepository = twoFactorRepository;
        _passwordResetCodeService = passwordResetCodeService;
        _jwtConfig = jwtConfig;
        _emailService = emailService;
    }

    public async Task<int> RegisterUserAsync(UserRegisterDto userDto)
    {
        var user = new UserTable()
        {
            Name = userDto.Name,
            Email = userDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password)
        };

        return await _userRepository.CreateAsync(user);
    }

    public async Task<UserDto> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
            return null;

        return new UserDto
        {
            Name = user.Name,
            Email = user.Email
        };
    }

    public async Task<bool> DeleteAccountAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return false;


        await _userRepository.DeleteUserAsync(userId);
        return true;
    }

    public async Task<UserDto> GetUserByEmailAsync(string email)
    {
        var user = await _userRepository.GetByEmailAsync(email);

        if (user == null)
            return null;

        return new UserDto
        {
            Name= user.Name,
            Email = user.Email
        };
    }

    public async Task<TokenResponse> LoginAsync(UserLoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
            return null;

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            return null;

        var accessToken = GenerateJwtToken(user);
        var refreshToken = GenerateRefreshToken();

        await _refreshTokenRepository.StoreRefreshTokenAsync(user.Id, refreshToken);

        return new TokenResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<TokenResponse> RefreshTokenAsync(string refreshToken)
    {
        var userId = await _refreshTokenRepository.GetUserIdByRefreshTokenAsync(refreshToken);
        if (userId == null)
            return null;

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null)
            return null;

        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();
        await _refreshTokenRepository.UpdateRefreshTokenAsync(user.Id, newRefreshToken);

        return new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<bool> UpdateUserProfileAsync(int id, UserUpdateDto updateDto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        await _userRepository.UpdateUserProfileAsync(id, updateDto.Name, updateDto.Email);
        return true;
    }

    private string GenerateJwtToken(UserTable user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtConfig.JwtSecret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private string GenerateRefreshToken()
    {
        // Генеруємо 32 байти випадкових даних, конвертуємо в Base64 рядок.
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }

    public async Task<bool> RequestPasswordResetCodeAsync(ForgotPasswordRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
            return true;

        var code = await _passwordResetCodeService.GenerateAndStoreResetCodeAsync(request.Email);

        string subject = "Відновлення паролю";
        string body = $"Ваш код для відновлення паролю: {code}. Він дійсний протягом 10 хвилин.";


        await _emailService.SendEmailAsync(request.Email, subject, body);
        
        Console.WriteLine($"Reset code for {request.Email}: {code}");

        return true;
    }

    // Метод для скидання паролю через код
    public async Task<bool> ResetPasswordWithCodeAsync(ResetPasswordCodeDto resetDto)
    {
        // Перевіряємо, чи код валідний для даного email
        bool valid = await _passwordResetCodeService.ValidateResetCodeAsync(resetDto.Email, resetDto.Code);
        if (!valid)
            return false;

        var user = await _userRepository.GetByEmailAsync(resetDto.Email);
        if (user == null)
            return false;

        // Хешуємо новий пароль за допомогою BCrypt
        string newPasswordHash = BCrypt.Net.BCrypt.HashPassword(resetDto.NewPassword);
        await _userRepository.UpdateUserPasswordAsync(user.Id, newPasswordHash);

        // Видаляємо код з кешу, оскільки він використаний
        _passwordResetCodeService.RemoveResetCode(resetDto.Email);

        return true;
    }

    public async Task<(string SecretBase32, byte[] QrCodePng)> BeginTwoFactorSetupAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var secretBytes = KeyGeneration.GenerateRandomKey(20);
        var secretBase32 = Base32Encoding.ToString(secretBytes);

        await _twoFactorRepository.UpsertTwoFactorSecretAsync(userId, secretBase32, false);

        var issuer = "CrypticGateway";
        var account = user.Email;
        var otpauth = $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(account)}" +
                      $"?secret={secretBase32}&issuer={Uri.EscapeDataString(issuer)}";

        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(otpauth, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrData);
        var qrCodePng = qrCode.GetGraphic(20);

        return (secretBase32, qrCodePng);
    }

    public async Task<bool> ConfirmTwoFactorAsync(int userId, string code)
    {
        var twoFactor = await _twoFactorRepository.GetTwoFactorByUserIdAsync(userId);
        if (twoFactor == null || twoFactor.IsEnabled) return false;

        var secretBytes = Base32Encoding.ToBytes(twoFactor.Secret);
        var totp = new Totp(secretBytes);
        bool isValid = totp.VerifyTotp(code, out long _, VerificationWindow.RfcSpecifiedNetworkDelay);
        if (!isValid) return false;

        await _twoFactorRepository.UpdateTwoFactorEnabledAsync(userId, true, DateTime.UtcNow);
        return true;
    }

    public async Task<bool> VerifyTwoFactorCodeAsync(int userId, string code)
    {
        var twoFactor = await _twoFactorRepository.GetTwoFactorByUserIdAsync(userId);
        if (twoFactor == null || !twoFactor.IsEnabled) return false;

        var secretBytes = Base32Encoding.ToBytes(twoFactor.Secret);
        var totp = new Totp(secretBytes);
        bool isValid = totp.VerifyTotp(code, out long _, VerificationWindow.RfcSpecifiedNetworkDelay);
        if (!isValid) return false;

        await _twoFactorRepository.UpdateTwoFactorEnabledAsync(userId, true, DateTime.UtcNow);
        return true;
    }

    public async Task DisableTwoFactorAsync(int userId)
    {
        await _twoFactorRepository.DisableTwoFactorAsync(userId);
    }

    public async Task<bool> IsTwoFactorEnabledAsync(int userId)
    {
        var twoFactor = await _twoFactorRepository.GetTwoFactorByUserIdAsync(userId);
        return twoFactor != null && twoFactor.IsEnabled;
    }

    public async Task<UserDto> ValidateCredentialsAsync(string email, string password)
    {
        var userTable = await _userRepository.GetByEmailAsync(email);
        if (userTable == null)
            return null;

        // Перевірка хешованого пароля (припустимо, що в userTable.PasswordHash міститься bcrypt-хеш)
        bool passwordMatches = BCrypt.Net.BCrypt.Verify(password, userTable.PasswordHash);
        if (!passwordMatches)
            return null;

        return new UserDto
        {
            Id = userTable.Id,       // додайте Id у UserDto, якщо його немає
            Name = userTable.Name,
            Email = userTable.Email
        };
    }

    public async Task<TokenResponse> GenerateTokensAsync(UserDto user)
    {
        // Припустимо, що ви створюєте JWT так:
        var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtConfig.JwtSecret);
        var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, user.Email),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.Name)
                }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        string jwt = tokenHandler.WriteToken(token);

        // Тут можете додати й refreshToken (генерація, збереження в БД тощо).
        string refreshToken = Guid.NewGuid().ToString(); // приклад

        return new TokenResponse
        {
            AccessToken = jwt,
            RefreshToken = refreshToken
        };
    }
}
