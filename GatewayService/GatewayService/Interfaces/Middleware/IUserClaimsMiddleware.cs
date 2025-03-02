namespace GatewayService.Interfaces.Middleware;

public interface IUserClaimsMiddleware
{
    Task InvokeAsync(HttpContext context);
}
