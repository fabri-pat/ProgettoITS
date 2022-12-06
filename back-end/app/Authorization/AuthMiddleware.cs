
using System.Security.Claims;
using app.Dtos;
using app.MyException;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;

namespace app.Authorization
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
                var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

                /* try
                {
                    if (token != null)
                        tokenService.ValidateAccessToken(token);

                    throw new SecurityTokenException("Bearer token not received");
                }
                catch (SecurityTokenException)
                {
                    context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

                    if (refreshToken == null)
                        throw new UserNeedToLoginException();

                    var Tokens = await userRepository.RefreshTokenAsync(refreshToken);

                    SetRefreshToken(Tokens.RefreshToken, context);
                } */

                if (token == null)
                {
                    context.Request.Cookies.TryGetValue("refreshToken", out var refreshToken);

                    if (refreshToken == null)
                        throw new UserNeedToLoginException();

                    var Tokens = await userRepository.RefreshTokenAsync(refreshToken);

                    context.Request.Headers.Cookie = Tokens.RefreshToken.Token;
                    context.Request.Headers.Authorization = "Bearer " + Tokens.JwtToken;
                    SetResponseCookieRefreshToken(Tokens.RefreshToken, context);
                }
            }
            await requestDelegate(context);
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

