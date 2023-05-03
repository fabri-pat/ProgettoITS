using app.Dtos;
using app.Entities;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace app.Repositories
{
    public class UserRepository : MongoRepository<User, Guid>, IUserRepository
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
                id: Guid.NewGuid(),
                name: user.Name,
                surname: user.Surname,
                username: user.Username,
                email: user.Email,
                role: Role.User,
                password: passwordService.CreatePassword(user.Password)
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

        public async Task<User?> ForgotPasswordAsync(String email)
        {
            var user = await GetByExpressionAsync(x => x.Email == email);

            if (user != null)
            {
                user.ResetToken = tokenService.GenerateResetToken();
                await UpdateAsync(user);
            }

            return user != null ? user : null;
        }

        public async Task UpdatePasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            User? user = await GetByExpressionAsync(x => x.Email == resetPasswordRequestDto.Email);

            if (user == null)
                throw new ArgumentException("No user registrated with provided email.");

            user.Password = passwordService.CreatePassword(resetPasswordRequestDto.NewPassword);

            await UpdateAsync(user);
        }
    }
}