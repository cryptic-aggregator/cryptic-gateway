using GatewayService.Models.Dtos.PortfolioConfiguration.Requests;
using GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

namespace GatewayService.Interfaces.Services;

public interface IPortfolioGrpcService
{
    Task<PortfolioResponseModel> CreatePortfolioAsync(CreatePortfolioRequestModel request, int ownerId);
    Task<PortfolioResponseModel> GetPortfolioAsync(int id, int ownerId);
    Task<List<PortfolioResponseModel>> GetPortfoliosByOwnerAsync(int ownerId);
    Task<PortfolioResponseModel> UpdatePortfolioAsync(UpdatePortfolioRequestModel portfolio, int id, int ownerId);
    Task<ConnectWalletsResponseModel> ConnectWalletsAsync(ConnectWalletsRequestModel request, int id, int ownerId);
    Task<PortfolioInfoResponseModel> GetPortfolioInfoAsync(int id, int ownerId);
    Task<bool> DeletePortfolioAsync(int id, int ownerId);

    Task<bool> PatchWalletVisibility(int portfolioId, int walletId, int visibility);

    Task<List<WalletModel>> GetWalletsByPortfolioIdAsync(
        int portfolioId,
        WalletsFilterModel filters);

    Task<PortfolioCalculationResponseModel> GetPortfolioCalculationAsync(int portfolioId, int ownerId);

    Task<PortfolioTransactionsResponseModel> GetPortfolioTransactionsAsync(
        int portfolioId,
        int ownerId,
        GetPortfolioTransactionsRequestModel filters);
    
    Task<PortfolioInfoWithWalletsResponseModel> GetPortfolioInfoWithWalletsAsync(int portfolioId, int ownerId);

    Task<PortfolioCorrelationResponseModel>
        GetPortfolioCorrelationAsync(
            int portfolioId,
            int ownerId,
            PortfolioCorrelationRequestModel filters);
}