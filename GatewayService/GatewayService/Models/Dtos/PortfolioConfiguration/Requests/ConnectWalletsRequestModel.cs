using System.Text.Json.Serialization;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Requests;

public class ConnectWalletsRequestModel
{
    [JsonPropertyName("wallet_addresses")]
    public List<string> WalletAddresses { get; set; }
}