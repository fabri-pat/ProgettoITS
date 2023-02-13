
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace app.Dtos
{
    public record UserDto(
        Guid Id,
        String Name,
        String Surname,
        String Username,
        String Email
    );

    public record RegistrationRequestDto(
        String Name,
        String Surname,
        String Username,
        [EmailAddress(ErrorMessage = "Invalid mail address")]
        String Email,
        [DataType(DataType.Password)]
        [StringLength(18, ErrorMessage = "Must be between 8 and 18 characters", MinimumLength = 8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one number and one special character")]
        String Password
    );

    public record LoginRequestDto(
        String Username,
        String Password
        );

    public record LoginResponseDto(
        String Message,
        UserDto User,
        [property: JsonIgnore]
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
        RefreshTokenDto RefreshToken
    );

    public record ResetPasswordRequestDto(
        String Email
    )
    {
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must have at least one uppercase letter, one lowercase letter, one number and one special character")]
        public String NewPassword { get; init; } = default!;

        [Compare("NewPassword", ErrorMessage = "Confirm password and password don't match")]
        public String NewPasswordConfirm { get; set; } = default!;
    };
}