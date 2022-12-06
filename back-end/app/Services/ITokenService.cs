using System.Security.Claims;
using app.Dtos;
using app.Entities;

public interface ITokenService
{
    String CreateAccessToken(User user);
    RefreshTokenDto GenerateRefreshToken();
    byte[] EncryptRefreshToken(string refreshToken, byte[] hash);
    bool VerifyRefreshToken(String token, RefreshToken refreshToken);
    ClaimsPrincipal ValidateAccessToken(string jwtToken);
}