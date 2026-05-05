using System.Security.Claims;
using JudgeAPI.Application.Common.Dtos;
using JudgeAPI.Application.Features;
using JudgeAPI.Application.Features.Users.Dtos;
using JudgeAPI.Application.Features.Users.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JudgeAPI.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        // GET: api/user/93e138af-c72b-4c52-8e60-c794abceefce
        // Auth: no authentication required
        [HttpGet("{id:guid}", Name = "GetUserById")]
        public async Task<ActionResult<UserBaseDTO>> GetUserById(Guid id)
        {
            string? currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentId is null)
            {
                return BadRequest();
            }

            UserBaseDTO user = await _userService.GetUserByIdAsync(id.ToString(), currentId);
            return Ok(user);
        }

        // GET: api/user/me
        // Auth: Bearer [Token]
        [HttpGet("me")]
        public async Task<ActionResult<UserBaseDTO>> CurrentUser()
        {
            string? currentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (currentId is null)
            {
                return BadRequest();
            }

            return await _userService.GetCurrentUser(currentId);
        }

        // PUT: api/user/93e138af-c72b-4c52-8e60-c794abceefce
        // Auth: Bearer [Token]
        // Body (example):
        // {
        //   "FirstName": "John",
        //   "Email": "john@test.com",
        //   "LastName": "Doe",
        //   "University": "MIT"
        // }
        [HttpPut("{userid:guid}")]
        public async Task<ActionResult<UserBaseDTO>> UpdateUser(string userid, [FromBody] UserUpdateDTO userData)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new ArgumentNullException(nameof(userData));

            if (userid != userId)
            {
                throw new ArgumentException("No puede editar el profile de otro usuario");
            }

            UserBaseDTO userResponse = await _userService.UpdateUser(userData, userId);
            return Ok(userResponse);
        }

        // PUT: api/user/93e138af-c72b-4c52-8e60-c794abceefce/roles
        // Auth: Bearer [Token]
        // Body: 
        // {
        //   "Roles": ["Admin", "Student"]
        // } 
        [HttpPut("{userid:guid}/roles")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserBaseDTO>> UpdateUser(Guid userId, [FromBody] UserUpdateRolesDTO userUpdateRoles)
        {
            if (userUpdateRoles is null || userUpdateRoles.Roles.Count == 0)
            {
                return BadRequest();
            }

            UserBaseDTO userResponse = await _userService.UpdateUserRoles(userId, userUpdateRoles);
            return Ok(userResponse);
        }

        // GET: api/user/all
        // Auth: Bearer [Token]
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<UsersResponseDTO> GetAllUsers()
        {
            return await _userService.GetUsersAsync();
        }

        // DELETE: api/user/93e138af-c72b-4c52-8e60-c794abceefce
        // Auth: Bearer [Token]
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public IActionResult DeleteUser(Guid id)
        {
            // verificar Id de Admin
            // TODO: Baja lógica
            // TODO: Armar servicio
            return Ok();
        }

        // GET: api/user/roles
        // Auth: Bearer [Token]
        [Authorize(Roles = "Admin")]
        [HttpGet("roles")]
        public async Task<RolesResponseDTO> GetRoles()
        {
            return await _userService.GetRolesAsync();
        }

        // PUT api/user/change-password
        // Auth: Bearer [Token]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO dto)
        {
            // Claim del User que solicita el cambio
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
            {
                return BadRequest();
            }

            IdentityResultDTO result = await _userService.ChangePasswordAsync(dto, userId);

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
