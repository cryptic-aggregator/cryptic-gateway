using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class PortfolioTransactionsResponseModel
{
    [JsonPropertyName("portfolio")]
    [JsonProperty("portfolio")]
    public PortfolioResponseModel Portfolio { get; set; } = new PortfolioResponseModel();

    [JsonPropertyName("transactions")]
    [JsonProperty("transactions")]
    public List<WalletTransactionDto> Transactions { get; set; } = new List<WalletTransactionDto>();

    [JsonPropertyName("total")]
    [JsonProperty("total")]
    public int Total { get; set; }

    [JsonPropertyName("page")]
    [JsonProperty("page")]
    public int Page { get; set; }

    [JsonPropertyName("per_page")]
    [JsonProperty("per_page")]
    public int PerPage { get; set; }
}