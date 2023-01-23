using app.Dtos;
using app.Entities;
using MongoDB.Driver;

namespace app.Repositories
{
    public class UserRepository : MongoRepository<User>, IUserRepository
    {
        private const string collectionName = "Users";

        private readonly ITokenService tokenService;

        private readonly IPasswordService passwordService;

        public UserRepository(
            IServiceProvider serviceProvider,
            ITokenService tokenService,
            IPasswordService passwordService
            ) : base(serviceProvider.GetService<IMongoDatabase>()!, collectionName)
        {
            this.tokenService = tokenService;
            this.passwordService = passwordService;
        }

        public async Task<User> GetUserByUsernameAsync(String username)
        {
            return await GetByExpressionAsync(x => x.Username == username);
        }

        public async Task RegisterUserAsync(RegistrationRequestDto user)
        {
            if (user == null)
                throw new ArgumentNullException();

            if (await GetUserByUsernameAsync(user.Username) != null)
                throw new ArgumentException("Username is already in use");

            var userToSave = new User(
                Guid.NewGuid(),
                user.Name,
                user.Surname,
                user.Username,
                user.Email,
                passwordService.CreatePassword(user.Password),
                Role.User
            );

            await CreateAsync(userToSave);
        }

        public async Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginRequestDto)
        {
            User? user = await GetUserByUsernameAsync(loginRequestDto.Username);

            if (user == null)
                throw new ArgumentException("Username not valid.");

            if (!passwordService.VerifyPassword(loginRequestDto.Password, user.Password))
                throw new ArgumentException("Password not valid.");

            var updatedUser = await UpdateUserTokensAsync(user);

            return new LoginResponseDto(
                Message: $"{user.Username} has been logged.",
                User: user.AsDto(),
                JwtToken: updatedUser.JwtToken,
                RefreshToken: updatedUser.RefreshToken
            );
        }

        public async Task<RefreshTokenSuccessDto> RefreshTokenAsync(String refreshToken)
        {
            var users = await GetAllAsync();

            User? user = users
                .Where(x => tokenService.VerifyRefreshToken(refreshToken, x.RefreshToken)).FirstOrDefault();

            if (user == null)
                throw new ArgumentException("Invalid refresh token");

            if (user.RefreshToken.ExpirationDate < DateTime.Now)
                throw new ArgumentException("Token expired.");

            return await UpdateUserTokensAsync(user);
        }

        private async Task<RefreshTokenSuccessDto> UpdateUserTokensAsync(User user)
        {
            var accessToken = tokenService.CreateAccessToken(user);

            var refreshToken = tokenService.GenerateRefreshToken();

            user.RefreshToken = tokenService.EncryptRefreshToken(refreshToken);

            await UpdateAsync(user);

            return new RefreshTokenSuccessDto(
                JwtToken: accessToken,
                RefreshToken: refreshToken
            );
        }
    }
}