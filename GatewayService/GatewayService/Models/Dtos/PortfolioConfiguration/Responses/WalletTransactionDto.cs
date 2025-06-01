using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class WalletTransactionDto
{
    [JsonPropertyName("transaction_id")]
    [JsonProperty("transaction_id")]
    public long TransactionId { get; set; }

    [JsonPropertyName("wallet_id")]
    [JsonProperty("wallet_id")] 
    public int WalletId { get; set; }

    [JsonPropertyName("token")]
    [JsonProperty("token")]
    public TokenInfoDto Token { get; set; } = new TokenInfoDto();

    [JsonPropertyName("transaction_hash")]
    [JsonProperty("transaction_hash")]
    public string TransactionHash { get; set; } = string.Empty;

    [JsonPropertyName("from_address")]
    [JsonProperty("from_address")]
    public string FromAddress { get; set; } = string.Empty;

    [JsonPropertyName("to_address")]
    [JsonProperty("to_address")]
    public string ToAddress { get; set; } = string.Empty;

    [JsonPropertyName("amount")]
    [JsonProperty("amount")]
    public string Amount { get; set; } = "0";

    [JsonPropertyName("timestamp")]
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("transaction_type")]
    [JsonProperty("transaction_type")]
    public int TransactionType { get; set; }

    [JsonPropertyName("chain")]
    [JsonProperty("chain")]
    public string Chain { get; set; } = string.Empty;

    [JsonPropertyName("fee")]
    [JsonProperty("fee")]
    public string Fee { get; set; } = "0";

    [JsonPropertyName("status")]
    [JsonProperty("status")]
    public int Status { get; set; }
}