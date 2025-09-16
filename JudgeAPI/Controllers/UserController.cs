using JudgeAPI.Models.User;
using JudgeAPI.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JudgeAPI.Controllers
{
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
        public async Task<ActionResult<UserResponseDTO>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            return Ok(user);
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateUser(string id, [FromBody] UserUpdateDTO userData)
        {
            
            return Ok();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteUser(int id)
        {
            // TODO: Implement method logic
            return Ok();
        }

        [HttpGet]
        public IActionResult GetUsers()
        {
            // TODO: Implement method logic, check for admin role
            return Ok();
        }
    }
}
