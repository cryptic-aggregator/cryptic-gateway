using GatewayService.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace GatewayService.Services;

public class PasswordResetCodeService : IPasswordResetCodeService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _expiry = TimeSpan.FromMinutes(10);

    public PasswordResetCodeService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<string> GenerateAndStoreResetCodeAsync(string email)
    {
        var random = new Random();
        string code = random.Next(100000, 1000000).ToString("D6");
        _cache.Set(email, code, _expiry);
        return Task.FromResult(code);
    }

    public void RemoveResetCode(string email)
    {
        _cache.Remove(email);
    }

    public Task<bool> ValidateResetCodeAsync(string email, string code)
    {
        if (_cache.TryGetValue(email, out string storedCode))
        {
            return Task.FromResult(storedCode == code);
        }
        return Task.FromResult(false);
    }
}
