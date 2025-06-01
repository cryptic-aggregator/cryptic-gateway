using GatewayService.Database.Tables;

namespace GatewayService.Interfaces.Repositories;

public interface ITwoFactorRepository
{
    Task UpsertTwoFactorSecretAsync(int userId, string secret, bool isEnabled);
    Task<UserTwoFactorTable?> GetTwoFactorByUserIdAsync(int userId);
    Task UpdateTwoFactorEnabledAsync(int userId, bool isEnabled, DateTime? lastVerifiedAt);
    Task DisableTwoFactorAsync(int userId);
}
