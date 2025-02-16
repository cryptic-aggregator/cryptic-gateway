using GatewayService.Services;
namespace GatewayService.DI;

public static class ServiceDIConfiguation
{
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<UserService>();
    }
}