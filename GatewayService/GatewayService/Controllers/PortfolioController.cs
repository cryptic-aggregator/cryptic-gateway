using System.Security.Claims;
using GatewayService.Controllers.Base;
using GatewayService.Interfaces.Services;
using GatewayService.Models.Dtos.BlockchainInteraction.Requests;
using GatewayService.Models.Dtos.PortfolioConfiguration.Requests;
using GatewayService.Models.Dtos.PortfolioConfiguration.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers;

[ApiController]
[Route("api/portfolio")]
public class PortfolioController : BaseController
{
    //TODO change userId = 1 to user id from UserClaims
    private readonly IPortfolioGrpcService _portfolioGrpcService;

    public PortfolioController(IPortfolioGrpcService portfolioGrpcService)
    {
        _portfolioGrpcService = portfolioGrpcService;
    }
    
    [HttpPost]
    public async Task<IActionResult> CreatePortfolio([FromBody] CreatePortfolioRequestModel request)
    {
        var result = await _portfolioGrpcService.CreatePortfolioAsync(request, UserClaims.UserId);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPortfolio(int id)
    {
        var result = await _portfolioGrpcService.GetPortfolioAsync(id, UserClaims.UserId);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
    
    [HttpGet("list")]
    public async Task<IActionResult> GetPortfoliosByOwner()
    {
        var result = await _portfolioGrpcService.GetPortfoliosByOwnerAsync(UserClaims.UserId);
        return Ok(result);
    }

    [HttpPatch("{id}")]

    public async Task<IActionResult> UpdatePortfolio([FromBody] UpdatePortfolioRequestModel portfolio, int id)
    {
        var result = await _portfolioGrpcService.UpdatePortfolioAsync(portfolio, id, UserClaims.UserId);
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePortfolio(int id)
    {
        var success = await _portfolioGrpcService.DeletePortfolioAsync(id, UserClaims.UserId);
        if (success)
            return Ok();
        return BadRequest();
    }
    
    [HttpPost("{id}/connect-wallets")]
    public async Task<IActionResult> ConnectWallets([FromBody] ConnectWalletsRequestModel request, int id)
    {
        var result = await _portfolioGrpcService.ConnectWalletsAsync(request, id, UserClaims.UserId);
        return Ok(result);
    }
    
    [HttpGet("{id}/info")]
    public async Task<IActionResult> GetPortfolioInfo(int id)
    {
        var result = await _portfolioGrpcService.GetPortfolioInfoAsync(id, UserClaims.UserId);
        return Ok(result);
    }
    
    [HttpPatch("{portfolioId}/wallet/{walletId}")]
    public async Task<IActionResult> PatchWalletVisibility(
        int portfolioId, 
        int walletId, 
        [FromBody] PatchWalletVisibilityModel model)
    {
        if (model == null)
            return BadRequest("No body provided");
        
        var success = await _portfolioGrpcService.PatchWalletVisibility(portfolioId, walletId, (int)model.Visibility);

        if (!success)
            return NotFound("Wallet not found or no rows were updated.");

        return Ok();
    }
    
    [HttpGet("{portfolioId}/wallets/")]
    public async Task<IActionResult> GetAllWallets(int portfolioId)
    {
        if (portfolioId <= 0)
        {
            return BadRequest("Invalid portfolio ID");
        }

        var wallets = await _portfolioGrpcService.GetWalletsByPortfolioIdAsync(portfolioId);
        
        return Ok(wallets);
    }
}