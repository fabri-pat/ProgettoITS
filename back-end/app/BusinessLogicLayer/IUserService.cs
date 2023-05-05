using app.Dtos;
using app.Entities;

namespace app.BusinessLogicLayer
{
    public interface IUserService
    {
        Task ForgotPasswordAsync(string email);
        Task<IReadOnlyCollection<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid id);
        Task<User> GetUserByRefreshTokenAsync(string token);
        Task<User> GetUserByUsernameAsync(string username);
        Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginRequestDto);
        Task<RefreshTokenSuccessDto> RefreshTokenAsync(string refreshToken);
        Task RegisterUserAsync(RegistrationRequestDto user);
        Task UpdatePasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto);
    }
}
