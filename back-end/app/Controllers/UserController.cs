
using app.Authorization;
using app.Dtos;
using app.Entities;
using Microsoft.AspNetCore.Authorization;
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
        [MyAuthorize]
        public async Task<IEnumerable<UserDto>> GetUsersAsync()
        {
            var users = (await userRepository.GetUsersAsync());
            return users.Select(user => user.AsDto());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserAsync(Guid id)
        {
            var user = (await userRepository.GetUserAsync(id));
            if (user == null)
                return NotFound();
            return Ok(user.AsDto());
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponseDto>> RegisterUser(RegistrationRequestDto request)
        {
            try
            {
                var response = (await userRepository.CreateUserAsync(request));
                return Ok(new RegistrationResponseDto(response.AsDto(), true));
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> LoginUser(LoginRequestDto loginRequest)
        {
            try
            {
                var response = (await userRepository.LoginUserAsync(loginRequest));

                SetRefreshToken(response.RefreshToken);

                return Ok(response);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<RefreshTokenSuccessDto>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            RefreshTokenSuccessDto response;

            if (refreshToken == null)
                return Unauthorized("Refresh token not found. Please log.");

            try
            {
                response = await userRepository.RefreshTokenAsync(refreshToken);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }

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