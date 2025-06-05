using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

public class PortfolioResponseModel
{
    [JsonProperty("id")]
    public int Id { get; set; }
        
    [JsonProperty("name")]
    public string Name { get; set; }
        
    [JsonProperty("owner_id")]
    public int OwnerId { get; set; }
    
    [JsonProperty("created_at")]
    public long CreatedAt { get; set; }
}