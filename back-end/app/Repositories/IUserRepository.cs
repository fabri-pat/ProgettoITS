using app.Entities;
using app.Dtos;

namespace app.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task RegisterUserAsync(RegistrationRequestDto user);
        Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginRequestDto);
        Task<RefreshTokenSuccessDto> RefreshTokenAsync(string refreshToken);
        Task<User> GetUserByUsernameAsync(String username);
    }
}
