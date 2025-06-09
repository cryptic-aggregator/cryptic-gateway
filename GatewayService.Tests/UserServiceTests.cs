using GatewayService.Database.Tables;
using GatewayService.Interfaces.Config;
using GatewayService.Interfaces.Repositories;
using GatewayService.Interfaces.Services;
using GatewayService.Models;
using GatewayService.Models.Dtos;
using GatewayService.Services;
using Moq;
using OtpNet;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GatewayService.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private readonly Mock<ITwoFactorRepository> _twoFactorRepositoryMock;
        private readonly Mock<IPasswordResetCodeService> _passwordResetCodeServiceMock;
        private readonly Mock<IJwtConfiguration> _jwtConfigurationMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _twoFactorRepositoryMock = new Mock<ITwoFactorRepository>();
            _passwordResetCodeServiceMock = new Mock<IPasswordResetCodeService>();
            _jwtConfigurationMock = new Mock<IJwtConfiguration>();
            _emailServiceMock = new Mock<IEmailService>();

            // In the UserServiceTests constructor, update the JWT secret to be at least 32 characters
            _jwtConfigurationMock.SetupGet(x => x.JwtSecret)
                .Returns("supersecretkey12345678901234567890123"); // 32 characters

            _userService = new UserService(
                _userRepositoryMock.Object,
                _refreshTokenRepositoryMock.Object,
                _twoFactorRepositoryMock.Object,
                _passwordResetCodeServiceMock.Object,
                _jwtConfigurationMock.Object,
                _emailServiceMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldHashPasswordAndReturnId()
        {
            var registerDto = new UserRegisterDto { Name = "TestUser", Email = "test@example.com", Password = "Password123" };
            _userRepositoryMock
                .Setup(repo => repo.CreateAsync(It.IsAny<UserTable>()))
                .ReturnsAsync(1)
                .Callback<UserTable>(user =>
                {
                    Assert.NotEqual(registerDto.Password, user.PasswordHash);
                    Assert.Equal(registerDto.Email, user.Email);
                    Assert.Equal(registerDto.Name, user.Name);
                });

            var result = await _userService.RegisterUserAsync(registerDto);

            Assert.Equal(1, result);
            _userRepositoryMock.Verify(repo => repo.CreateAsync(It.IsAny<UserTable>()), Times.Once);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_ValidCredentials_ReturnsUserDto()
        {
            // Arrange
            var password = "Password123";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var userTable = new UserTable { Id = 5, Name = "Test", Email = "test@example.com", PasswordHash = hash };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(userTable.Email))
                .ReturnsAsync(userTable);

            // Act
            var result = await _userService.ValidateCredentialsAsync(userTable.Email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userTable.Id, result.Id);
            Assert.Equal(userTable.Name, result.Name);
            Assert.Equal(userTable.Email, result.Email);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_InvalidPassword_ReturnsNull()
        {
            // Arrange
            var password = "Password123";
            var hash = BCrypt.Net.BCrypt.HashPassword(password);
            var userTable = new UserTable { Id = 5, Name = "Test", Email = "test@example.com", PasswordHash = hash };
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(userTable.Email))
                .ReturnsAsync(userTable);

            // Act
            var result = await _userService.ValidateCredentialsAsync(userTable.Email, "WrongPass");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task ValidateCredentialsAsync_NonexistentUser_ReturnsNull()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((UserTable)null);

            // Act
            var result = await _userService.ValidateCredentialsAsync("nouser@example.com", "pass");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task IsTwoFactorEnabledAsync_ReturnsTrue_WhenEnabled()
        {
            // Arrange
            var dto = new UserTwoFactorTable { UserId = 1, Secret = "ABC", IsEnabled = true };
            _twoFactorRepositoryMock.Setup(r => r.GetTwoFactorByUserIdAsync(1)).ReturnsAsync(dto);

            // Act
            var result = await _userService.IsTwoFactorEnabledAsync(1);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsTwoFactorEnabledAsync_ReturnsFalse_WhenNotEnabledOrNull()
        {
            // Arrange
            _twoFactorRepositoryMock.Setup(r => r.GetTwoFactorByUserIdAsync(1)).ReturnsAsync((UserTwoFactorTable)null);
            Assert.False(await _userService.IsTwoFactorEnabledAsync(1));

            _twoFactorRepositoryMock.Setup(r => r.GetTwoFactorByUserIdAsync(2))
                .ReturnsAsync(new UserTwoFactorTable { UserId = 2, Secret = "XYZ", IsEnabled = false });

            // Act
            var result = await _userService.IsTwoFactorEnabledAsync(2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task VerifyTwoFactorCodeAsync_ValidCode_ReturnsTrueAndUpdates()
        {
            // Arrange: use zero-byte secret and mark as enabled
            var secretBytes = new byte[20];
            var secret = Base32Encoding.ToString(secretBytes);
            var table = new UserTwoFactorTable { UserId = 1, Secret = secret, IsEnabled = true };
            _twoFactorRepositoryMock.Setup(r => r.GetTwoFactorByUserIdAsync(1)).ReturnsAsync(table);

            // Generate current valid code
            var totp = new Totp(secretBytes);
            var code = totp.ComputeTotp();

            _twoFactorRepositoryMock.Setup(r => r.UpdateTwoFactorEnabledAsync(1, true, It.IsAny<DateTime>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var result = await _userService.VerifyTwoFactorCodeAsync(1, code);

            // Assert
            Assert.True(result);
            _twoFactorRepositoryMock.Verify(r => r.GetTwoFactorByUserIdAsync(1), Times.Once);
            _twoFactorRepositoryMock.Verify(r => r.UpdateTwoFactorEnabledAsync(1, true, It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task VerifyTwoFactorCodeAsync_InvalidCode_ReturnsFalse()
        {
            // Arrange
            var secret = Base32Encoding.ToString(KeyGeneration.GenerateRandomKey(20));
            var table = new UserTwoFactorTable { UserId = 1, Secret = secret, IsEnabled = false };
            _twoFactorRepositoryMock.Setup(r => r.GetTwoFactorByUserIdAsync(1)).ReturnsAsync(table);

            // Act
            var result = await _userService.VerifyTwoFactorCodeAsync(1, "000000");

            // Assert
            Assert.False(result);
            _twoFactorRepositoryMock.Verify(r => r.UpdateTwoFactorEnabledAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task GenerateTokensAsync_StoresRefreshTokenAndReturnsTokens()
        {
            // Arrange
            var userDto = new UserDto { Id = 7, Name = "User7", Email = "u7@example.com" };
            _refreshTokenRepositoryMock.Setup(r => r.StoreRefreshTokenAsync(7, It.IsAny<string>()))
                .Returns(Task.CompletedTask)
                .Verifiable();

            // Act
            var response = await _userService.GenerateTokensAsync(userDto);

            // Assert
            Assert.NotNull(response);
            Assert.False(string.IsNullOrEmpty(response.AccessToken));
            Assert.False(string.IsNullOrEmpty(response.RefreshToken));
            _refreshTokenRepositoryMock.Verify();
        }
    }
}