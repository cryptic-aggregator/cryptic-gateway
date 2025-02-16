﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GatewayService.Database.Tables;
using GatewayService.Interfaces.Repositories;
using GatewayService.Models;
using Microsoft.IdentityModel.Tokens;

namespace GatewayService.Services;

public class UserService //TODO use interface for better abstraction
{
    private readonly IUserRepository _userRepository;
    /*private readonly string _jwtSecret;*/ //TODO use ConfigService and create variable in launchSettings

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<int> RegisterUserAsync(UserRegisterDto userDto)
    {
        var user = new UserTable()
        {
            Name = userDto.Name,
            Email = userDto.Email,
            PasswordMd5 = CalculateMd5Hash(userDto.Password)
        };

        return await _userRepository.CreateAsync(user);
    }

    private Guid CalculateMd5Hash(string input)
    {
        using (var md5 = MD5.Create())
        {
            var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return new Guid(hashBytes);
        }
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

    public async Task<string> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
            return null;

        if (CalculateMd5Hash(loginDto.Password) != user.PasswordMd5)
            return null;

        return GenerateJwtToken(user);
    }

    private string GenerateJwtToken(UserTable user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes("_jwtSecret");

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
}
