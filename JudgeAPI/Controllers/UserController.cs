using JudgeAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace JudgeAPI.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        public UserController()
        {
            
        }

        [HttpGet("{id:alpha}")]
        public IActionResult GetUserById(string id)
        {
            // TODO: Implement method logic
            return Ok();
        }

        [HttpPut("{id:alpha}")]
        public IActionResult UpdateUser(string id, [FromBody] UserUpdateDTO userData)
        {
            // TODO: Implement method logic
            return Ok();
        }

        [HttpDelete("{id}")]
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
