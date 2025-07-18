using DisasterReport.Data.Models;
using DisasterReport.Services.Models.UserDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DisasterReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            if (users == null)
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }


        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllActiveUsers()
        {
            var users = await _userService.GetAllActiveUsersAsync();
            if (users == null)
            {
                return NotFound("No users found.");
            }
            return Ok(users);
        }


        [HttpGet("admins")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllAdmins()
        {
            var users = await _userService.GetAllAdminsAsync();
            if (users == null)
            {
                return NotFound("No admin found.");
            }
            return Ok(users);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }


        [HttpGet("email")]
        public async Task<ActionResult<UserDto>> GetUserByEmail([FromQuery] string email)
        {
            var user = await _userService.GetUsersByEmailAsync(email);
            if (user == null) return NotFound();

            return Ok(user);
        }


        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirst("id") ?? User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
                return NotFound();

            return Ok(user);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            await _userService.UpdateUserAsync(id, updateUserDto);
            return Ok("Updated successfully!");
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            //return NoContent();
            return Ok("Deleted successfully");
        }
    }
}
