using JudgeAPI.Models;
using JudgeAPI.Services;
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

        /// <summary>
        /// Registers a new user.
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<UserResponseDTO>> Register(UserCreateDTO dto)
        {
            var responseDTO = await _authService.RegisterAsync(dto);
            return Ok(responseDTO);
        }

        /// <summary>
        /// Logs in a user.
        /// </summary>
        [HttpPost("login")]
        public IActionResult Login()
        {
            // TODO: Implement login logic
            return Ok();
        }

        /// <summary>
        /// Logs out a user.
        /// </summary>
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // TODO: Implement logout logic
            return Ok();
        }

        /// <summary>
        /// Refreshes an authentication token.
        /// </summary>
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            // TODO: Implement token refresh logic
            return Ok();
        }

        /// <summary>
        /// Confirms a user's email address.
        /// </summary>
        [HttpPost("confirm-email")]
        public IActionResult ConfirmEmail()
        {
            // TODO: Implement email confirmation logic
            return Ok();
        }

        /// <summary>
        /// Initiates the password reset process.
        /// </summary>
        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword()
        {
            // TODO: Implement forgot password logic
            return Ok();
        }

        /// <summary>
        /// Resets a user's password.
        /// </summary>
        [HttpPost("reset-password")]
        public IActionResult ResetPassword()
        {
            // TODO: Implement password reset logic
            return Ok();
        }
    }
}
