using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ITNBaja.Services;

namespace ITNBaja.Attributes
{
    public class TokenAuthAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tokenService = context.HttpContext.RequestServices.GetRequiredService<TokenService>();
            
            // Get token from Authorization header
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            
            if (authHeader != null && authHeader.StartsWith("Token "))
            {
                var token = authHeader.Substring("Token ".Length).Trim();
                
                if (tokenService.ValidateToken(token, out string username))
                {
                    // Token is valid, set user context
                    context.HttpContext.Items["Username"] = username;
                    context.HttpContext.Items["IsAuthenticated"] = true;
                    return;
                }
            }
            
            // Token is invalid or missing
            context.Result = new UnauthorizedObjectResult(new { Message = "Invalid or missing token" });
        }
    }
}