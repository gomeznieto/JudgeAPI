using JudgeAPI.Application.Features;
using JudgeAPI.Application.Features.Auth.Dtos;
using JudgeAPI.Application.Features.Auth.Iterfaces;
using JudgeAPI.Application.Features.Users.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace JudgeAPI.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(
            IAuthService authService
            )
        : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        // POST: api/auth/register
        // Auth: no authentication required
        [HttpPost("register")]
        public async Task<ActionResult<UserBaseDTO>> Register(UserCreateDTO dto)
        {
            UserBaseDTO responseDTO = await _authService.RegisterAsync(dto);
            return Ok(responseDTO);
        }

        // POST: api/auth/login
        // Auth: no authentication required
        // Example JSON request body:
        // {
        //     "email": "user@example.com",
        //     "password": "securepassword123"
        // }
        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDTO>> Login([FromBody] LoginRequestDTO request)
        {
            TokenResponseDTO tokenResponse = await _authService.LoginAsync(request);
            return Ok(tokenResponse);
        }

        // TODO: Implementar los métodos que faltan
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok();
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken(TokenRequestDTO dto)
        {
            string? currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Console.WriteLine($"Refresh token request received: {dto.RefreshToken}");

            if (currentUserId is null)
            {
                return BadRequest("User ID not found in claims.");
            }

            TokenResponseDTO result = await _authService.RefreshTokenAsync(dto, currentUserId);
            return Ok(result);
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
