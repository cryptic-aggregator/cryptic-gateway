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

namespace GatewayService.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IJwtConfiguration _jwtConfig;

    public UserService(IUserRepository userRepository, IRefreshTokenRepository refreshTokenRepository, IJwtConfiguration jwtConfig)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _jwtConfig = jwtConfig;
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

        // (Опціонально: можна реалізувати ротацію refresh token – генерувати новий замість використання того самого)
        var newAccessToken = GenerateJwtToken(user);
        var newRefreshToken = GenerateRefreshToken();
        await _refreshTokenRepository.UpdateRefreshTokenAsync(user.Id, newRefreshToken);

        return new TokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
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
}
