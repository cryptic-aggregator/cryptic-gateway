using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Requests;

public class PortfolioCorrelationRequestModel
{
    [JsonPropertyName("symbol")]
    [JsonProperty("symbol")]
    public string Symbol { get; set; } = string.Empty;

    [JsonPropertyName("from_ts")]
    [JsonProperty("from_ts")]
    public long FromTs { get; set; }

    [JsonPropertyName("to_ts")]
    [JsonProperty("to_ts")]
    public long ToTs { get; set; }

    [JsonPropertyName("points_count")]
    [JsonProperty("points_count")]
    public int PointsCount { get; set; } = 12;
}