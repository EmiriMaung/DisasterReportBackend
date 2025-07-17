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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers([FromQuery] UserFilterOptions options)
        {
            var users = await _userService.GetAllUsersAsync(options);
            return Ok(users);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllActiveUsers([FromQuery] UserFilterOptions options)
        {
            options.OnlyBlacklisted = false;
            var users = await _userService.GetAllUsersAsync(options);
            return Ok(users);
        }

        [HttpGet("blacklisted")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAllBlacklistedUsers([FromQuery] UserFilterOptions options)
        {
            options.OnlyBlacklisted = true;
            var users = await _userService.GetAllUsersAsync(options);
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

        [HttpGet("blacklisted/{id}")]
        public async Task<ActionResult<UserDto>> GetBlacklistedUserById(Guid id)
        {
            var user = await _userService.GetBlacklistedUserByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            await _userService.UpdateUserAsync(updateUserDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
