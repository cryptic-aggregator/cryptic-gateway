using System.Text.Json.Serialization;
using Cryptic_Domain.Enums.Portfolio;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Requests;

public class WalletConnectEntityModel
{
    [JsonPropertyName("name")]
    [JsonProperty("name")]
    public string? Name { get; set; }
        
    [JsonPropertyName("caip_address")]
    [JsonProperty("caip_address")]
    public string? CaipAddress { get; set; }
        
    [JsonPropertyName("connector")]
    [JsonProperty("connector")]
    public string? Connector { get; set; }
        
    [JsonPropertyName("connection_type")]
    [JsonProperty("connection_type")]
    public WalletConnectionType ConnectionType { get; set; }
        
    [JsonPropertyName("wallet_address")]
    [JsonProperty("wallet_address")]
    public string WalletAddress { get; set; }
}

public class ConnectWalletsRequestModel
{
    [JsonPropertyName("wallets")]
    [JsonProperty("wallets")]
    public List<WalletConnectEntityModel> Wallets { get; set; }
}