using app.Entities;
using app.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace app.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task RegisterUserAsync(RegistrationRequestDto user);
        Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginRequestDto);
        Task<RefreshTokenSuccessDto> RefreshTokenAsync(string refreshToken);
        Task<User> GetUserByUsernameAsync(String username);
        Task<User?> ForgotPasswordAsync(String email);
        Task UpdatePasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto);
    }
}
