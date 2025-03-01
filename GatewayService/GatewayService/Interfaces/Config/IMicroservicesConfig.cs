namespace GatewayService.Interfaces.Config;

public interface IMicroservicesConfig
{
    public string BlockchainInteractionConnString { get; }
    
    public string PortfolioConfigurationConnString { get; }
}