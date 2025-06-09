using GatewayService.Models.Dtos;

namespace GatewayService.Interfaces.Services;

public interface IUserService
{
    Task<int> RegisterUserAsync(UserRegisterDto userDto);
    Task<UserDto> GetUserByIdAsync(int id);
    Task<UserDto> GetUserByEmailAsync(string email);
    //Task<TokenResponse> LoginAsync(UserLoginDto loginDto);
    Task<TokenResponse> RefreshTokenAsync(string refreshToken);
    Task<bool> DeleteAccountAsync(int userId);
    Task<bool> UpdateUserProfileAsync(int id, UserUpdateDto updateDto);
    Task<bool> RequestPasswordResetCodeAsync(ForgotPasswordRequestDto request);
    Task<bool> ResetPasswordWithCodeAsync(ResetPasswordCodeDto resetDto);
    Task<(string SecretBase32, byte[] QrCodePng)> BeginTwoFactorSetupAsync(int userId);
    Task<bool> ConfirmTwoFactorAsync(int userId, string code);
    Task<bool> VerifyTwoFactorCodeAsync(int userId, string code);
    Task DisableTwoFactorAsync(int userId);
    Task<bool> IsTwoFactorEnabledAsync(int userId);
    Task<UserDto> ValidateCredentialsAsync(string email, string password);
    Task<TokenResponse> GenerateTokensAsync(UserDto user);
}
