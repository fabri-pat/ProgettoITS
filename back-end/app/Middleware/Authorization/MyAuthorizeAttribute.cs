
using Microsoft.AspNetCore.Mvc.Filters;

namespace app.Middleware.Authorization
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MyAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public string? Roles { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var token = context.HttpContext.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

            var user = context.HttpContext.User;

            if (Roles != null)
            {
                if (!user.IsInRole(Roles))
                    throw new UnauthorizedAccessException("You can't access to specified resource. Restricted");
            }
        }
    }
}