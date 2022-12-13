
using app.Dtos;
using app.MyException;
using app.Repositories;
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
            IUserRepository userRepository,
            ITokenService tokenService)
        {
            bool hasAuthorizeAttribute = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.Any(x => x is MyAuthorizeAttribute) ?? false;

            if (hasAuthorizeAttribute)
            {
                context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

                if (refreshToken == null)
                    throw new UserNeedToLoginException("Please login to access to the request resource.");

                var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

                if (token == null)
                    token = await TryGetNewJwtTokenUsingRefreshTokenAsync(refreshToken, userRepository, context);

                try
                {
                    context.User = tokenService.ValidateAccessToken(token);
                }
                catch (SecurityTokenExpiredException)
                {
                    token = await TryGetNewJwtTokenUsingRefreshTokenAsync(refreshToken, userRepository, context);
                    context.User = tokenService.ValidateAccessToken(token);
                }
            }
            await requestDelegate(context);
        }

        private async Task<string> TryGetNewJwtTokenUsingRefreshTokenAsync(String refreshToken, IUserRepository userRepository, HttpContext context)
        {
            var Tokens = await userRepository.RefreshTokenAsync(refreshToken);

            SetResponseCookieRefreshToken(Tokens.RefreshToken, context);

            return Tokens.JwtToken;
        }

        private void SetResponseCookieRefreshToken(RefreshTokenDto refreshToken, HttpContext context)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpirationDate,
                Secure = true
            };
            context.Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
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

