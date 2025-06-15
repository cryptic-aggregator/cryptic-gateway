using System.Text.Json.Serialization;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;


public class PortfolioPnlResponseModel
{
    [JsonPropertyName("points")]
    public List<PnlPointDto> Points { get; set; } = new();

    [JsonPropertyName("success")]
    public bool Success { get; set; }
}