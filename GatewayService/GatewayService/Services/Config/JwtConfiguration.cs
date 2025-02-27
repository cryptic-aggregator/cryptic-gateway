using GatewayService.Interfaces.Config;

namespace GatewayService.Services.Config;

public class JwtConfiguration : IJwtConfiguration
{
    public JwtConfiguration()
    {
        JwtSecret = Environment.GetEnvironmentVariable("JwtSecret") ?? throw new Exception("JWT secret is missing in environment variables.");
    }

    public string JwtSecret { get; set; }
}
