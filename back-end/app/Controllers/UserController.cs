
using app.Dtos;
using app.Entities;
using app.Middleware.Authorization;
using app.Repositories;
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

            SetRefreshToken(response.RefreshToken);

            return Ok(response);
        }

        [HttpPost("logout")]
        public ActionResult Logout()
        {
            Response.Cookies.Delete("refreshToken");
            return Ok("You are now logged out");
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenSuccessDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
                return Unauthorized("Refresh token not found. Please log.");

            var response = await userRepository.RefreshTokenAsync(refreshToken);

            SetRefreshToken(response.RefreshToken);
            return Ok(response);
        }

        private void SetRefreshToken(RefreshTokenDto refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = refreshToken.ExpirationDate,
                Secure = true
            };
            Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
        }
    }
}