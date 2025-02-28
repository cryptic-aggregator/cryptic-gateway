using GatewayService.Models.Dtos.BlockchainInteraction.Responses;

namespace GatewayService.Interfaces.Services;

public interface IWalletGrpcService
{
    Task<WalletResponseModel> GetWalletCoinsAsync(List<string> address);
}