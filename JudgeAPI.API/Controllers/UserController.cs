namespace JudgeAPI.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JudgeAPI.Application.Features;
using System.Security.Claims;


[ApiController]
[Route("api/users")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id:guid}", Name = "GetUserById")]
    public async Task<ActionResult<UserBaseDTO>> GetUserById(string id)
    {
        var currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(currentId is null) return BadRequest();

        var user = await _userService.GetUserByIdAsync(id, currentId);
        return Ok(user);
    }

    // -- RETORNA USUARIO ACTUAL LOGEADO -- //
    [HttpGet("me")]
    public async Task<ActionResult<UserBaseDTO>> currentUser()
    {
        
        return await _userService.GetCurrectUser(); 
    }

    // -- UPDATE DE USUARIO -- //
    [HttpPut("{userid:guid}")]
    public async Task<ActionResult<UserBaseDTO>> UpdateUser(string userid, [FromBody] UserUpdateDTO userData)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userData == null) throw new ArgumentNullException(nameof(userData));

        if (userid != userId) throw new ArgumentException(nameof(userData));

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
        var result = await _userService.ChangePasswordAsync(dto);

        if (!result.Succeeded)
        {
            return BadRequest(new
                    {
                    Errors = result.Errors.Select(e => e)
                    });
        }

        return Ok(new { Error = false, Message = "Password changed successfully" });
    }
}

