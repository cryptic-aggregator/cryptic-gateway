using System.Text.Json.Serialization;
using Cryptic_Domain.Enums.Portfolio;
using Newtonsoft.Json;

namespace GatewayService.Models.Dtos.BlockchainInteraction.Requests;

public class PatchWalletVisibilityModel
{
    [JsonProperty("visibility")]
    [JsonPropertyName("visibility")]
    public WalletVisibility Visibility { get; set; }
}