
using System.Security.Claims;
using app.Entities;
using Microsoft.AspNetCore.Mvc.Filters;

namespace app.Middleware.Authorization
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MyAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public Role Roles { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!IsInRole(user, Roles))
                throw new UnauthorizedAccessException(
                    "You can't access to specified resource. Restricted");
        }

        private static bool IsInRole(ClaimsPrincipal user, Role role)
        {
            var roles = user.Claims
                        .Where(x => x.Type == ClaimTypes.Role)
                        .FirstOrDefault();
            Role userRole;

            if(roles == null)
                userRole = Role.Guest;
            else    
                Enum.TryParse<Role>(roles?.Value, true, out userRole);

            if (userRole >= role)
                return true;

            return false;
        }
    }
}