using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class PortfolioCorrelationResponseModel
{
    [JsonPropertyName("points")]
    [JsonProperty("points")]
    public List<PortfolioCorrelationPointDto> Points { get; set; } 
        = new List<PortfolioCorrelationPointDto>();
}