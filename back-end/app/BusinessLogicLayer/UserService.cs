using app.Dtos;
using app.Entities;
using app.Services;
using MongoDbBaseRepository;

namespace app.BusinessLogicLayer
{
    public class UserService : IUserService
    {
        /* private readonly IUserRepository userRepository; */
         private readonly IRepository<User, Guid> userRepository; 
        private readonly IEMailService emailService;
        private readonly ITokenService tokenService;
        private readonly IPasswordService passwordService;

        /* public UserService(
                    IUserRepository userRepository,
                    IEMailService emailService,
                    ITokenService tokenService,
                    IPasswordService passwordService)
        {
            this.userRepository = userRepository;
            this.emailService = emailService;
            this.tokenService = tokenService;
            this.passwordService = passwordService;
        } */

        public UserService(
                    IRepository<User, Guid> userRepository,
                    IEMailService emailService,
                    ITokenService tokenService,
                    IPasswordService passwordService)
        {
            this.userRepository = userRepository;
            this.emailService = emailService;
            this.tokenService = tokenService;
            this.passwordService = passwordService;
        }

        public async Task<IReadOnlyCollection<User>> GetAllUsersAsync()
        {
            return await userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByRefreshTokenAsync(String token)
        {
            return await userRepository.GetByExpressionAsync(x => x.ResetToken.Token == token);
        }


        public async Task<User> GetUserByIdAsync(Guid id)
        {
            return await userRepository.GetByIdAsync(id);
        }

        public async Task<User> GetUserByUsernameAsync(String username)
        {
            return await userRepository.GetByExpressionAsync(x => x.Username == username);
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

            await userRepository.CreateAsync(userToSave);
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
            var users = await userRepository.GetAllAsync();

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

            await userRepository.UpdateAsync(user);

            return new RefreshTokenSuccessDto(
                JwtToken: accessToken,
                RefreshToken: refreshToken
            );
        }

        public async Task ForgotPasswordAsync(String email)
        {
            var user = await userRepository.GetByExpressionAsync(x => x.Email == email);

            if (user != null)
            {
                user.ResetToken = tokenService.GenerateResetToken();
                await userRepository.UpdateAsync(user);

                await emailService.SendAsync(
                to: user.Email,
                subject: "Reset password",
                body: String.Format("<a href='https://localhost:7155/api/v1/User/reset-password?token={0}'>Clicca qui</a href> per cambiare la password.", user.ResetToken.Token)
                );
            }
        }

        public async Task UpdatePasswordAsync(ResetPasswordRequestDto resetPasswordRequestDto)
        {
            User? user = await userRepository.GetByExpressionAsync(x => x.Email == resetPasswordRequestDto.Email);

            if (user == null)
                throw new ArgumentException("No user registrated with provided email.");

            user.Password = passwordService.CreatePassword(resetPasswordRequestDto.NewPassword);

            await userRepository.UpdateAsync(user);
        }
    }
}