using Cryptic.BlockchainInteraction.Rpc;
using GatewayService.Interfaces.Config;

namespace GatewayService.DI;

public static class MicroservicesDIConfigure
{
    public static void ConfigureMicroservices(this IServiceCollection services, IMicroservicesConfig cfg)
    {
        var customHandler = new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
        };
        
        services.AddGrpcClient<WalletService.WalletServiceClient>(opt =>
        {
            opt.Address = new Uri("http://" + cfg.BlockchainInteractionConnString);
        }).ConfigurePrimaryHttpMessageHandler(() => customHandler);
    }
}