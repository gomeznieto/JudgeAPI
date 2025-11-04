using JudgeAPI.Models.User;
using JudgeAPI.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace JudgeAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ICurrentUserService _current;

        public UserController(IUserService userService, ICurrentUserService current)
        {
            _userService = userService;
            _current = current;
        }

        [HttpGet("{id:guid}", Name = "GetUserById")]
        [ProducesResponseType(typeof(UserPublicDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserPrivateDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserAdminDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserBaseDTO>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            return Ok(user);
        }

        [HttpGet("me")]
        [ProducesResponseType(typeof(UserPublicDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserPrivateDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserAdminDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserBaseDTO>> currentUser()
        {
            return await _userService.GetCurrectUser();
        }

        [HttpPut("{userid:guid}")]
        [ProducesResponseType(typeof(UserPrivateDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserBaseDTO>> UpdateUser(string userid, [FromBody] UserUpdateDTO userData)
        {
            if (userData == null) throw new ArgumentNullException(nameof(userData));

            if (userid != userData.Id) throw new ArgumentException(nameof(userData));

            var userResponse = await _userService.UpdateUser(userData);

            return Ok(userResponse);
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteUser(int id)
        {
            // TODO: Baja lógica
            return Ok();
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            // TODO: Pensar en este endpoint para obtener los mejores 10 de cada ejercicios o un ranking general basado en submissions
            return Ok();
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _userService.ChangePasswordAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Errors = result.Errors.Select(e => e.Description)
                });
            }

            return Ok(new { Message = "Password changed successfully" });
        }
    }
}
