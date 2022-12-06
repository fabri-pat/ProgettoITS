
using System.Text.Json.Serialization;

namespace app.Dtos
{
    public record UserDto(
        Guid Id,
        String Name,
        String Surname,
        String Username
    );

    public record RegistrationRequestDto(
        String Name,
        String Surname,
        String Username,
        String Password
    );

    public record RegistrationResponseDto(UserDto User, Boolean result);

    public record LoginRequestDto(
        String Username,
        String Password
        );

    public record LoginResponseDto(
        String Message,
        String JwtToken,
        [property: JsonIgnore]
        RefreshTokenDto RefreshToken
    );

    public record RefreshTokenDto(
        String Token,
        DateTime CreatedDate,
        DateTime ExpirationDate
    );

    public record RefreshTokenSuccessDto(
        String JwtToken,
        [property: JsonIgnore]
        RefreshTokenDto RefreshToken
    );
}