
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PizzaShop.service.Interfaces;

namespace PizzaShop.service.Implementation

{
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public CustomAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Inject JwtService to use in Middleware.
            var jwtService = context.HttpContext.RequestServices.GetService(typeof(IJwtService)) as IJwtService;

            // Get the token from Cookie
            var cookieService = new CookieService();
            var token = cookieService.GetJWTToken(context.HttpContext.Request);

            // Validate Token
            var principal = jwtService?.ValidateToken(token ?? "");
            if (principal == null)
            {
                context.Result = new RedirectToActionResult("Login", "Login", null);
                return;
            }

            context.HttpContext.User = principal;

            if (_roles.Length > 0)
            {

                var userRole = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                if (!_roles.Contains(userRole))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Login", null);
                }
            }
        }
    }
}