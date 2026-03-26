using JudgeAPI.Application.Features;
using JudgeAPI.Application.Features.Auth.Dtos;
using JudgeAPI.Application.Features.Auth.Iterfaces;
using JudgeAPI.Application.Features.Users.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace JudgeAPI.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(
            IAuthService authService
            )
        : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        public async Task<ActionResult<UserBaseDTO>> Register(UserCreateDTO dto)
        {
            UserBaseDTO responseDTO = await _authService.RegisterAsync(dto);
            return Ok(responseDTO);
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDTO>> Login([FromBody] LoginRequestDTO request)
        {
            TokenResponseDTO tokenResponse = await _authService.LoginAsync(request);
            return Ok(tokenResponse);
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok();
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken()
        {
            return Ok();
        }

        [HttpPost("confirm-email")]
        public IActionResult ConfirmEmail()
        {
            return Ok();
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return Ok();
        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword()
        {
            return Ok();
        }
    }

}
