
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

        public UserController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet]
        [MyAuthorize(Roles = Role.Guest)]
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = (await userRepository.GetAllAsync());
            return users.Select(user => user.AsDto());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserAsync(Guid id)
        {
            var user = (await userRepository.GetByIdAsync(id));
            if (user == null)
                return NotFound();
            return Ok(user.AsDto());
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(RegistrationRequestDto request)
        {
            await userRepository.RegisterUserAsync(request);
            return Ok($"{request.Username} is now registered.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> LoginUser(LoginRequestDto loginRequest)
        {
            var response = (await userRepository.LoginUserAsync(loginRequest));

            CookieService.SetResponseCookies(this.HttpContext, response.JwtToken, response.RefreshToken.Token);

            return Ok(response);
        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            Response.Cookies.Delete("accessToken");
            return Ok("You are now logged out");
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
                return Unauthorized("Refresh token not found. Please log.");

            var response = await userRepository.RefreshTokenAsync(refreshToken);

            CookieService.SetResponseCookies(this.HttpContext, response.JwtToken, response.RefreshToken.Token);

            return Ok();
        }
    }
}