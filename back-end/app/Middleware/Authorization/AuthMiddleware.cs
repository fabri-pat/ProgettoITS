
using app.BusinessLogicLayer;
using app.MyException;
using app.Services;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;

namespace app.Middleware.Authorization
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate requestDelegate;
        public AuthMiddleware(RequestDelegate next)
        {
            this.requestDelegate = next;
        }

        public async Task InvokeAsync(
            HttpContext context,
            IUserService userService,
            ITokenService tokenService)
        {
            bool hasAuthorizeAttribute = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.Any(x => x is MyAuthorizeAttribute) ?? false;

            if (hasAuthorizeAttribute)
            {
                context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
                context.Request.Cookies.TryGetValue("accessToken", out var accessToken);

                if (refreshToken == null)
                    throw new UserNeedToLoginException("Please login to access to the request resource.");

                try
                {
                    context.User = tokenService.ValidateAccessToken(accessToken!);
                }
                catch (SecurityTokenExpiredException)
                {
                    await TryGetNewJwtTokenUsingRefreshTokenAsync(refreshToken, userService, context);
                }
                catch (ArgumentNullException)
                {
                    await TryGetNewJwtTokenUsingRefreshTokenAsync(refreshToken, userService, context);
                }
            }
            await requestDelegate(context);
        }

        private async Task<string> TryGetNewJwtTokenUsingRefreshTokenAsync(String refreshToken, IUserService userService, HttpContext context)
        {
            var Tokens = await userService.RefreshTokenAsync(refreshToken);

            CookieService.SetResponseCookies(context, Tokens.JwtToken, Tokens.RefreshToken.Token);

            return Tokens.JwtToken;
        }
    }

    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthMiddleware>();
        }
    }

}

