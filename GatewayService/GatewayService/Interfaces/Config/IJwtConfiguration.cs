namespace GatewayService.Interfaces.Config;

public interface IJwtConfiguration
{
    string JwtSecret { get; set; }
}
