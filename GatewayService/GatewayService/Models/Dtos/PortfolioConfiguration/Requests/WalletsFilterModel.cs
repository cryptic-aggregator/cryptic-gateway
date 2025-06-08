using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Requests;

public class WalletsFilterModel
{
    [JsonPropertyName("search")]
    [JsonProperty("search")]
    public string? Search { get; set; }

    [JsonPropertyName("networks")]
    [JsonProperty("networks")]
    public List<string>? Networks { get; set; }
}