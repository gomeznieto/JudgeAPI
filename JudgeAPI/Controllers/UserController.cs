using JudgeAPI.Models.User;
using JudgeAPI.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        // -- RETORNA USUARIO ACTUAL LOGEADO -- //
        [HttpGet("me")]
        [ProducesResponseType(typeof(UserPublicDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserPrivateDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(UserAdminDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserBaseDTO>> currentUser()
        {
          return await _userService.GetCurrectUser() ; 
        }

        // -- UPDATE DE USUARIO -- //
        [HttpPut("{userid:guid}")]
        [ProducesResponseType(typeof(UserPrivateDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserBaseDTO>> UpdateUser(string userid, [FromBody] UserUpdateDTO userData)
        {
          if (userData == null) throw new ArgumentNullException(nameof(userData));

          if (userid != userData.Id) throw new ArgumentException(nameof(userData));

          var userResponse = await _userService.UpdateUser(userData);

          return Ok(userResponse);
        }
        
        // -- UPDATE ROL DE USUARIO -- //
        [HttpPut("{userid:guid}/roles")]
        [ProducesResponseType(typeof(UserPrivateDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<UserBaseDTO>> UpdateUser(string userid, [FromBody] UserUpdateRolesDTO userUpdateRoles)
        {
          if (userUpdateRoles == null) throw new ArgumentNullException(nameof(userUpdateRoles));
          if (userUpdateRoles.Id != userid) throw new ArgumentNullException(nameof(userUpdateRoles));

          var userResponse = await _userService.UpdateUserRoles(userUpdateRoles);

          return Ok(userResponse);
        }

        // ---- GET ALL USERS ---- //
        [HttpGet("all")]
        public async Task<UsersResponseDTO> getAllUsers()
        {
          return await _userService.GetUsersAsync();
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
        
        [HttpGet("roles")]
        public async Task<RolesResponseDTO> GetRoles()
        {
          return await _userService.GetRolesAsync();
        }

        // -- CAMBIAR PASSWORD -- //
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
          Console.WriteLine($"[PASSEORD] {dto.OldPassword} {dto.NewPassword}");
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

          return Ok(new { Error = false, Message = "Password changed successfully" });
        }
    }
}
