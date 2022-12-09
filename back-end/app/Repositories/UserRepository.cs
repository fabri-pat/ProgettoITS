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
            FilterDefinition<User> filter = filterDefinitionBuilder.Eq(
                entity => entity.Username, username
            );

            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> CreateAsync(RegistrationRequestDto user)
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
                passwordService.CreatePassword(user.Password),
                "User"
            );
            
            await dbCollection.InsertOneAsync(userToSave);

            return userToSave;
        }

        public async Task<LoginResponseDto> LoginUserAsync(LoginRequestDto loginRequestDto)
        {
            FilterDefinition<User> filter = filterDefinitionBuilder.Eq(
                entity => entity.Username, loginRequestDto.Username
            );

            User? user = await dbCollection.Find(filter).FirstOrDefaultAsync();

            if (user == null)
                throw new ArgumentException("Username not valid.");

            if (!passwordService.VerifyPassword(loginRequestDto.Password, user.Password))
                throw new ArgumentException("Password not valid.");

            var token = tokenService.CreateAccessToken(user);

            var refreshToken = tokenService.GenerateRefreshToken();

            await UpdateRefreshTokenAsync(filter, refreshToken);

            return new LoginResponseDto(
                Message: "User has been logged.",
                JwtToken: token,
                RefreshToken: refreshToken
            );
        }

        public async Task<RefreshTokenSuccessDto> RefreshTokenAsync(String refreshToken)
        {
            FilterDefinition<User> filter = filterDefinitionBuilder.Empty;
            var users = await dbCollection.Find(filter).ToListAsync();

            User? user = users
                .Where(x => tokenService.VerifyRefreshToken(refreshToken, x.RefreshToken)).FirstOrDefault();

            if (user == null)
                throw new ArgumentException("Invalid refresh token");

            if (user.RefreshToken.ExpirationDate < DateTime.Now)
                throw new ArgumentException("Token expired.");

            var _accessToken = tokenService.CreateAccessToken(user);
            var _refreshToken = tokenService.GenerateRefreshToken();

            filter = filterDefinitionBuilder.Eq(x => x.Username, user.Username);

            await UpdateRefreshTokenAsync(filter, _refreshToken);

            return new RefreshTokenSuccessDto(
                JwtToken: _accessToken,
                RefreshToken: _refreshToken
            );
        }

        private async Task<bool> UpdateRefreshTokenAsync(FilterDefinition<User> filter, RefreshTokenDto refreshToken)
        {
            var hashCode = BitConverter.GetBytes(refreshToken.CreatedDate.GetHashCode());

            var encryptedToken = tokenService.EncryptRefreshToken(refreshToken.Token, hashCode);

            var newRefreshToken = new RefreshToken(
                encryptedToken,
                hashCode,
                refreshToken.CreatedDate, refreshToken.ExpirationDate
            );

            var update = updateDefinitionBuilder.Set("RefreshToken", newRefreshToken);

            var result = await dbCollection.UpdateOneAsync(filter, update);

            return result.IsAcknowledged;
        }
    }
}