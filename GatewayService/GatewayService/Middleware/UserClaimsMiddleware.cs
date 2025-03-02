using System.Reflection;
using GatewayService.Interfaces.Middleware;
using GatewayService.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;

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
        var controllerActionDescriptor = context
            .GetEndpoint()?
            .Metadata
            .GetMetadata<ControllerActionDescriptor>();

        var methodData = controllerActionDescriptor?.MethodInfo;
        
        var isAnonymous = methodData?.GetCustomAttribute<AllowAnonymousAttribute>() != null;

        if (!isAnonymous)
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
        
        await _next(context);
    }
}
