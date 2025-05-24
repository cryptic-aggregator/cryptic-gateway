using System.Text.Json.Serialization;
using Cryptic_Domain.Enums.Portfolio;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Requests;

public class WalletConnectEntityModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
        
    [JsonPropertyName("caip_address")]
    public string CaipAddress { get; set; }
        
    [JsonPropertyName("connector")]
    public string Connector { get; set; }
        
    [JsonPropertyName("connection_type")]
    public WalletConnectionType ConnectionType { get; set; }
        
    [JsonPropertyName("wallet_address")]
    public string WalletAddress { get; set; }
}

public class ConnectWalletsRequestModel
{
    [JsonPropertyName("wallets")]
    public List<WalletConnectEntityModel> Wallets { get; set; }
}