
using app.Dtos;
using app.Entities;
using app.Middleware.Authorization;
using app.Repositories;
using app.Services;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userRepository;
        private readonly IEMailService eMailService;

        public UserController(IUserRepository userRepository, IEMailService eMailService)
        {
            this.userRepository = userRepository;
            this.eMailService = eMailService;
        }

        [HttpGet]
        [MyAuthorize(Roles = Role.Guest)]
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            return (await userRepository.GetAllAsync()).Select(user => user.AsDto());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserAsync(Guid id)
        {
            var user = (await userRepository.GetByIdAsync(id));
            if (user == null)
                return NotFound();
            return user.AsDto();

        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync(RegistrationRequestDto request)
        {
            await userRepository.RegisterUserAsync(request);
            return Ok($"{request.Username} is now registered.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> LoginUserAsync(LoginRequestDto loginRequest)
        {
            var response = (await userRepository.LoginUserAsync(loginRequest));

            CookieService.SetResponseCookies(this.HttpContext, response.JwtToken, response.RefreshToken.Token);

            return response;
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Delete("accessToken");
            return Ok("You are now logged out");
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
                return Unauthorized("Refresh token not found. Please log.");

            var response = await userRepository.RefreshTokenAsync(refreshToken);

            CookieService.SetResponseCookies(this.HttpContext, response.JwtToken, response.RefreshToken.Token);

            return Ok();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(String email)
        {
            if (String.IsNullOrEmpty(email))
                return BadRequest("Null or empty email provided");

            User? user = await userRepository.ForgotPasswordAsync(email);

            if (user != null)
                await eMailService.SendAsync(
                to: user.Email,
                subject: "Reset password",
                body: String.Format("<a href='https://localhost:7155/api/v1/User/reset-password?token={0}'>Clicca qui</a href> per cambiare la password.", user.ResetToken.Token)
                );

            return Ok("Email send successfully");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(String token)
        {
            User? user = await userRepository.GetByExpressionAsync(x => x.ResetToken.Token == token);

            if (user == null || ((DateTime.Now.ToUniversalTime()) > user.ResetToken.ExpireDate))
                return BadRequest();

            return Ok("Valid Token");
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePasswordAsync(ResetPasswordRequestDto resetPasswordRequest)
        {
            await userRepository.UpdatePasswordAsync(resetPasswordRequest);

            return Ok("Password changed successfully.");
        }
    }
}