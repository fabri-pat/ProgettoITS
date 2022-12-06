using app.Dtos;
using app.Entities;
using MongoDB.Driver;

namespace app.Repositories
{
    public class UserRepository : IUserRepository
    {
        private const string collectionName = "Users";

        private readonly IMongoCollection<User> dbCollection;

        private readonly FilterDefinitionBuilder<User> filterDefinitionBuilder = Builders<User>.Filter;

        private readonly UpdateDefinitionBuilder<User> updateDefinitionBuilder = Builders<User>.Update;

        private readonly ITokenService tokenService;

        private readonly IPasswordService passwordService;

        public UserRepository(
            ITokenService tokenService,
            IPasswordService passwordService
            )
        {
            var settings = MongoClientSettings.FromConnectionString(
                "mongodb+srv://admin:se6676SSdFW1Z2gn@cluster0.oncf9mc.mongodb.net/?retryWrites=true&w=majority"
            );
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            var client = new MongoClient(settings);
            var database = client.GetDatabase("MyProject");
            dbCollection = database.GetCollection<User>(collectionName);

            this.tokenService = tokenService;
            this.passwordService = passwordService;
        }

        public async Task<IReadOnlyCollection<User>> GetUsersAsync()
        {
            return await dbCollection.Find(filterDefinitionBuilder.Empty).ToListAsync<User>();
        }

        public async Task<User> GetUserAsync(Guid id)
        {
            FilterDefinition<User> filter = filterDefinitionBuilder.Eq(
                entity => entity.Id, id
            );
            return await dbCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<User> CreateUserAsync(RegistrationRequestDto user)
        {
            if (user == null)
                throw new ArgumentNullException();

            FilterDefinition<User> filter = filterDefinitionBuilder.Eq(
                entity => entity.Username, user.Username
            );

            if (await dbCollection.Find(filter).FirstOrDefaultAsync() != null)
            {
                throw new ArgumentException("Username already in use.");
            }

            var userToSave = new User(Guid.NewGuid(),
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

        public async Task<User?> GetUserByUsernameAsync(String username)
        {
            FilterDefinition<User> filter = filterDefinitionBuilder.Eq(
                entity => entity.Username, username
            );

            User? user = await dbCollection.Find(filter).FirstOrDefaultAsync();

            return user;
        }
    }
}