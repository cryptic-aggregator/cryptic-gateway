namespace GatewayService.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task StoreRefreshTokenAsync(int userId, string refreshToken);
    Task<string> GetRefreshTokenAsync(int userId);
    Task<int?> GetUserIdByRefreshTokenAsync(string refreshToken);
    Task UpdateRefreshTokenAsync(int userId, string newRefreshToken);
}
