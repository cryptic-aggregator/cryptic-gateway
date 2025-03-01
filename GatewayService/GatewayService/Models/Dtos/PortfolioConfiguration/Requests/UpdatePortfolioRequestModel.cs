using System.Text.Json.Serialization;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Requests;

public class UpdatePortfolioRequestModel
{
    [JsonPropertyName("name")]
    public string Name { get; set; }
}