using GatewayService.Interfaces.Services;
using GatewayService.Models.Dtos.PortfolioConfiguration.Requests;
using GatewayService.Models.Dtos.PortfolioConfiguration.Responses;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers;

[ApiController]
[Route("api/portfolio")]
public class PortfolioController : ControllerBase
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
        var result = await _portfolioGrpcService.CreatePortfolioAsync(request);
        return Ok(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPortfolio(int id)
    {
        var result = await _portfolioGrpcService.GetPortfolioAsync(id, 1);
        if (result == null)
            return NotFound();
        return Ok(result);
    }
    
    [HttpGet("list")]
    public async Task<IActionResult> GetPortfoliosByOwner()
    {
        var result = await _portfolioGrpcService.GetPortfoliosByOwnerAsync(1);
        return Ok(result);
    }

    [HttpPatch("{id}")]

    public async Task<IActionResult> UpdatePortfolio([FromBody] UpdatePortfolioRequestModel portfolio, int id)
    {
        var result = await _portfolioGrpcService.UpdatePortfolioAsync(portfolio, id, 1);
        return Ok(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePortfolio(int id)
    {
        var success = await _portfolioGrpcService.DeletePortfolioAsync(id, 1);
        if (success)
            return Ok();
        return BadRequest();
    }
    
    [HttpPost("{id}/connect-wallets")]
    public async Task<IActionResult> ConnectWallets([FromBody] ConnectWalletsRequestModel request, int id)
    {
        var result = await _portfolioGrpcService.ConnectWalletsAsync(request, id, 1);
        return Ok(result);
    }
    
    [HttpGet("{portfolioId}/info")]
    public async Task<IActionResult> GetPortfolioInfo(int portfolioId)
    {
        var result = await _portfolioGrpcService.GetPortfolioInfoAsync(portfolioId, 1);
        return Ok(result);
    }
}