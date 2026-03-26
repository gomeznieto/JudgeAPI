using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JudgeAPI.Application.Features;
using System.Security.Claims;
using JudgeAPI.Application.Features.Users.Dtos;
using JudgeAPI.Application.Features.Users.Interfaces;
using JudgeAPI.Application.Common.Dtos;

namespace JudgeAPI.API.Controllers
{

    [ApiController]
    [Route("api/users")]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [HttpGet("{id:guid}", Name = "GetUserById")]
        public async Task<ActionResult<UserBaseDTO>> GetUserById(string id)
        {
            string? currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentId is null)
            {
                return BadRequest();
            }

            UserBaseDTO user = await _userService.GetUserByIdAsync(id, currentId);
            return Ok(user);
        }

        // -- RETORNA USUARIO ACTUAL LOGEADO -- //
        [HttpGet("me")]
        public async Task<ActionResult<UserBaseDTO>> CurrentUser()
        {
            return await _userService.GetCurrectUser();
        }

        // -- UPDATE DE USUARIO -- //
        [HttpPut("{userid:guid}")]
        public async Task<ActionResult<UserBaseDTO>> UpdateUser(string userid, [FromBody] UserUpdateDTO userData)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new ArgumentNullException(nameof(userData));

            if (userid != userId)
            {
                throw new ArgumentException("No puede editar el profile de otro usuario");
            }

            UserBaseDTO userResponse = await _userService.UpdateUser(userData);
            return Ok(userResponse);
        }

        // -- UPDATE ROL DE USUARIO -- //
        [HttpPut("{userid:guid}/roles")]
        public async Task<ActionResult<UserBaseDTO>> UpdateUser(string userid, [FromBody] UserUpdateRolesDTO userUpdateRoles)
        {
            if (userUpdateRoles is null)
            {
                return BadRequest();
            }

            if (userUpdateRoles.Id != userid)
            {
                throw new ArgumentException("No puede modificar el rol de otro usuario");
            }

            UserBaseDTO userResponse = await _userService.UpdateUserRoles(userUpdateRoles);
            return Ok(userResponse);
        }

        // ---- GET ALL USERS ---- //
        [HttpGet("all")]
        public async Task<UsersResponseDTO> GetAllUsers()
        {
            return await _userService.GetUsersAsync();
        }

        // DAR DE BAJA USUARIOS: EN FRONT YA ESTA ARMADO
        [HttpDelete("{id:guid}")]
        public IActionResult DeleteUser(int id)
        {
            // TODO: Baja lógica
            return Ok();
        }

        // -- LISTADO DE ROLES --// 
        [HttpGet("roles")]
        public async Task<RolesResponseDTO> GetRoles()
        {
            return await _userService.GetRolesAsync();
        }

        // -- CAMBIAR PASSWORD -- //
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            IdentityResultDTO result = await _userService.ChangePasswordAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    Errors = result.Errors!.Select(static e => e)
                });
            }

            return Ok(new { Error = false, Message = "Password changed successfully" });
        }
    }
}
