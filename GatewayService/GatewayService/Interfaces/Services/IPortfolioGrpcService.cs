using GatewayService.Models.Dtos.PortfolioConfiguration.Requests;
using GatewayService.Models.Dtos.PortfolioConfiguration.Responses;

namespace GatewayService.Interfaces.Services;

public interface IPortfolioGrpcService
{
    Task<PortfolioResponseModel> CreatePortfolioAsync(CreatePortfolioRequestModel request);
    Task<PortfolioResponseModel> GetPortfolioAsync(int id, int ownerId);
    Task<List<PortfolioResponseModel>> GetPortfoliosByOwnerAsync(int ownerId);
    Task<PortfolioResponseModel> UpdatePortfolioAsync(UpdatePortfolioRequestModel portfolio, int id, int ownerId);
    Task<ConnectWalletsResponseModel> ConnectWalletsAsync(ConnectWalletsRequestModel request, int id, int ownerId);
    Task<PortfolioInfoResponseModel> GetPortfolioInfoAsync(int id, int ownerId);
    Task<bool> DeletePortfolioAsync(int id, int ownerId);
}