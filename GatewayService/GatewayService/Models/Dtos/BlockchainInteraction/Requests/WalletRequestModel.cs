using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.BlockchainInteraction.Requests;

public class WalletRequestModel
{
    [JsonProperty("addresses")]
    [JsonPropertyName("addresses")]
    public List<string> Address { get; set; }
}