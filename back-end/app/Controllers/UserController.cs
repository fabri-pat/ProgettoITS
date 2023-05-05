
using app.BusinessLogicLayer;
using app.Dtos;
using app.Entities;
using app.Middleware.Authorization;
using app.Services;
using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [MyAuthorize(Roles = Role.Guest)]
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            return (await userService.GetAllUsersAsync()).Select(user => user.AsDto());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserAsync(Guid id)
        {
            var user = (await userService.GetUserByIdAsync(id));
            if (user == null)
                return NotFound();
            return user.AsDto();

        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUserAsync(RegistrationRequestDto request)
        {
            await userService.RegisterUserAsync(request);
            return Ok($"{request.Username} is now registered.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> LoginUserAsync(LoginRequestDto loginRequest)
        {
            var response = (await userService.LoginUserAsync(loginRequest));

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

            var response = await userService.RefreshTokenAsync(refreshToken);

            CookieService.SetResponseCookies(this.HttpContext, response.JwtToken, response.RefreshToken.Token);

            return Ok();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync(String email)
        {
            if (String.IsNullOrEmpty(email))
                return BadRequest("Null or empty email provided");

            await userService.ForgotPasswordAsync(email);

            return Ok("Email send successfully");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync(String token)
        {
            User? user = await userService.GetUserByRefreshTokenAsync(token);

            if (user == null || ((DateTime.Now.ToUniversalTime()) > user.ResetToken.ExpireDate))
                return BadRequest();

            return Ok("Valid Token");
        }

        [HttpPost("update-password")]
        public async Task<IActionResult> UpdatePasswordAsync(ResetPasswordRequestDto resetPasswordRequest)
        {
            await userService.UpdatePasswordAsync(resetPasswordRequest);

            return Ok("Password changed successfully.");
        }
    }
}