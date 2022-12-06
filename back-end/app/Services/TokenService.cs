using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using app.Dtos;
using app.Entities;
using Microsoft.IdentityModel.Tokens;

namespace app.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public String CreateAccessToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(
                    this.configuration.GetSection("AppSettings:Token").Value)
            );

            var credential = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddSeconds(120),
                signingCredentials: credential
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public ClaimsPrincipal ValidateAccessToken(string jwtToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes(
                    this.configuration.GetSection("AppSettings:Token").Value
                )
            );

            ClaimsPrincipal claimsPrincipal = tokenHandler.ValidateToken(
                jwtToken,
                new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(
                            this.configuration.GetSection("AppSettings:Token").Value
                        )
                    ),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                },
                out SecurityToken validatedToken
            );

            return claimsPrincipal;
        }

        public RefreshTokenDto GenerateRefreshToken()
        {
            var refreshToken = new RefreshTokenDto
            (
                Token: Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                CreatedDate: DateTime.Now,
                ExpirationDate: DateTime.Now.AddDays(7)
            );

            return refreshToken;
        }

        public byte[] EncryptRefreshToken(string refreshToken, byte[] hash)
        {
            var hmac = new HMACSHA512(hash);

            var refreshTokenHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(refreshToken));

            return refreshTokenHash;
        }

        public bool VerifyRefreshToken(String token, RefreshToken refreshToken)
        {
            var key = refreshToken.Sst;

            var hmac = new HMACSHA512(key);

            var refreshTokenHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));

            return refreshTokenHash.SequenceEqual(refreshToken.Token);
        }
    }
}