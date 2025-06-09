using System.Text.Json.Serialization;
using Cryptic.PortfolioAnalytic.Models.Requests;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Requests;

public class GetPortfolioTransactionsRequestModel
{
    [JsonPropertyName("page")]
    [JsonProperty("page")]
    public int Page { get; set; } = 1;

    [JsonPropertyName("per_page")]
    [JsonProperty("per_page")]
    public int PerPage { get; set; } = 10;

    [JsonPropertyName("transaction_type")]
    [JsonProperty("transaction_type")]
    public TransactionTypeFilter TransactionType { get; set; }

    [JsonPropertyName("date_from")]
    [JsonProperty("dateFrom")]
    public long? DateFrom { get; set; }
    
    [JsonPropertyName("date_to")]
    [JsonProperty("dateTo")]
    public long? DateTo { get; set; }

    [JsonPropertyName("search")]
    [JsonProperty("search")]
    public string? Search { get; set; } = string.Empty;
}