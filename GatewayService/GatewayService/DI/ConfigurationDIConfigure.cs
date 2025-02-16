using Cryptic_Domain.Database.Config.Interfaces;
using Cryptic_Domain.Database.Interfaces;
using Cryptic_Domain.Services;
using GatewayService.Services.Config;

namespace GatewayService.DI;

public static class ConfigurationDIConfigure
{
    public static void InjectConfiguration(this IServiceCollection services, ConfigService config)
    {
        services.AddSingleton<IDatabaseConfiguration>(config);
        services.AddSingleton<IDatabaseConnectionService, DatabaseConnectionService>();
    }
}