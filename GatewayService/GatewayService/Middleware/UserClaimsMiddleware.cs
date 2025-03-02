using GatewayService.Interfaces.Middleware;
using GatewayService.Models;
using System.Security.Claims;

namespace GatewayService.Middleware;

public class UserClaimsMiddleware : IUserClaimsMiddleware
{
    private readonly RequestDelegate _next;

    public UserClaimsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
        {
            var claimsIdentity = context.User.Identity as ClaimsIdentity;
            if (claimsIdentity != null)
            {
                var userIdStr = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(userIdStr, out int userId))
                {
                    var email = claimsIdentity.FindFirst(ClaimTypes.Email)?.Value;
                    var name = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;

                    var userClaims = new UserClaims
                    {
                        UserId = userId,
                        Email = email,
                        Name = name
                    };

                    context.Items["UserClaims"] = userClaims;
                }
                else
                {
                    throw new Exception("Invalid userId");
                }
            }
            
            await _next(context);
        }
        
        context.Response.StatusCode = 403;
    }
}
