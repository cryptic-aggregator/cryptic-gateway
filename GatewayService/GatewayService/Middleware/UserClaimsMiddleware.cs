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
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
        }

        await _next(context);
    }
}
