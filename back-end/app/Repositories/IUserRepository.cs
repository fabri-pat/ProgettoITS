using app.Entities;
using app.Dtos;

public interface IUserRepository
{
    Task<User> CreateUserAsync(RegistrationRequestDto user);
    Task<User> GetUserAsync(Guid id);
    Task<IReadOnlyCollection<User>> GetUsersAsync();
    Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginRequestDto);
    Task<RefreshTokenSuccessDto> RefreshTokenAsync(string refreshToken);
    Task<User?> GetUserByUsernameAsync(String username);
}