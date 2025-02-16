using GatewayService.Database.Repositories;
using GatewayService.Interfaces.Repositories;

namespace GatewayService.DI;

public static class ReposDIConfigure
{
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }
}