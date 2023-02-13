using System.Security.Claims;
using app.Dtos;
using app.Entities;

public interface ITokenService
{
    String CreateAccessToken(User user);
    RefreshTokenDto GenerateRefreshToken();
    RefreshToken EncryptRefreshToken(RefreshTokenDto refreshToken);
    bool VerifyRefreshToken(String token, RefreshToken refreshToken);
    ClaimsPrincipal ValidateAccessToken(string jwtToken);
    ResetToken GenerateResetToken();
}