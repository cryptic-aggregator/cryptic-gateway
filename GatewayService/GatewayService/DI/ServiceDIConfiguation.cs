using GatewayService.Interfaces.Services;
using GatewayService.Services;
using GatewayService.Services.ControllerSup.BlockchainInteraction;

namespace GatewayService.DI;

public static class ServiceDIConfiguation
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IWalletGrpcService, WalletGrpcService>();
    }
}