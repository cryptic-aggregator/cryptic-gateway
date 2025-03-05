namespace GatewayService.Interfaces.Services;

public interface IPasswordResetCodeService
{
    Task<string> GenerateAndStoreResetCodeAsync(string email);
    Task<bool> ValidateResetCodeAsync(string email, string code);
    void RemoveResetCode(string email);
}
