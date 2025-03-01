using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Requests;

public class ConnectWalletsRequestModel
{
    [JsonProperty("wallet_addresses")]
    [JsonPropertyName("wallet_addresses")]
    public List<string> WalletAddresses { get; set; }
}