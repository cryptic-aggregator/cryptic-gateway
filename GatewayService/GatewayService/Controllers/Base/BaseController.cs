using GatewayService.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace GatewayService.Controllers.Base;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (HttpContext.Items.ContainsKey("UserClaims"))
        {
            UserClaims = HttpContext.Items["UserClaims"] as UserClaims;
            base.OnActionExecuting(context);
        }
    }
    
    protected UserClaims UserClaims { get; private set; }
}