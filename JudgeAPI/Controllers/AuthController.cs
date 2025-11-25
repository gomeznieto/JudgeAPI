using JudgeAPI.Models.Auth;
using JudgeAPI.Models.User;
using JudgeAPI.Services.Ath;
using Microsoft.AspNetCore.Mvc;

namespace JudgeAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(
            IAuthService authService    
        )
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserBaseDTO>> Register(UserCreateDTO dto)
        {
            var responseDTO = await _authService.RegisterAsync(dto);
            return Ok(responseDTO);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest request)
        {
            var tokenResponse = await _authService.LoginAsync(request);   
            return Ok(tokenResponse);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // TODO: Implement logout logic
            return Ok();
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            // TODO: Implement token refresh logic
            return Ok();
        }

        [HttpPost("confirm-email")]
        public IActionResult ConfirmEmail()
        {
            // TODO: Implement email confirmation logic
            return Ok();
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword()
        {
            // TODO: Implement forgot password logic
            return Ok();
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword()
        {
            // TODO: Implement password reset logic
            return Ok();
        }
    }
}
