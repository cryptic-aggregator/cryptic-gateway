using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class BalanceGraphResponseModel
{
    [JsonPropertyName("points")]
    [JsonProperty("points")]
    public List<BalancePointDto> Points { get; set; } = new List<BalancePointDto>();
}