using System.Text.Json.Serialization;
using Cryptic_Domain.Enums.Portfolio;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Requests;

public class ConnectWalletsRequestModel
{
    [JsonProperty("wallet_addresses")]
    [JsonPropertyName("wallet_addresses")]
    public List<string> WalletAddresses { get; set; }
    
    [JsonProperty("connection_type")]
    [JsonPropertyName("connection_type")]
    public WalletConnectionType ConnectionType { get; set; }
}