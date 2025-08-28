using DisasterReport.Services.Models.Common;
using DisasterReport.Services.Models.UserDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
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

        //[HttpGet("all/paginated")]
        //public async Task<ActionResult<PaginatedResult<UserDto>>> GetPaginatedUsers([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        //{
        //    var result = await _userService.GetPaginatedUsersAsync(page, pageSize);
        //    return Ok(result);
        //}


        //[HttpGet("all")]
        //public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
        //{
        //    var users = await _userService.GetAllUsersAsync();
        //    if (users == null)
        //    {
        //        return NotFound("No users found.");
        //    }
        //    return Ok(users);
        //}

        [HttpGet("normal/paginated")]
        public async Task<ActionResult<PaginatedResult<UserDto>>> GetPaginatedNormalUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchQuery = null,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] string? sortOrder = "desc"
        )
        {
            var result = await _userService.GetPaginatedNormalUsersAsync(page, pageSize, searchQuery, sortBy, sortOrder);
            return Ok(result);
        }


        [HttpGet("active/paginated")]
        public async Task<ActionResult<PaginatedResult<UserDto>>> GetPaginatedActiveUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchQuery = null,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] string? sortOrder = "desc"
            )
        {
            var result = await _userService.GetPaginatedActiveUsersAsync(page, pageSize, searchQuery, sortBy, sortOrder);
            return Ok(result);
        }


        //[HttpGet("active")]
        //public async Task<ActionResult<IEnumerable<UserDto>>> GetAllActiveUsers()
        //{
        //    var users = await _userService.GetAllActiveUsersAsync();
        //    if (users == null)
        //    {
        //        return NotFound("No users found.");
        //    }
        //    return Ok(users);
        //}


        [HttpGet("admins/paginated")]
        public async Task<ActionResult<PaginatedResult<UserDto>>> GetPaginatedAdmins(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchQuery = null,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] string? sortOrder = "desc"
        )
        {
            var result = await _userService.GetPaginatedAdminsAsync(page, pageSize, searchQuery, sortBy, sortOrder);
            return Ok(result);
        }



        //[HttpGet("admins")]
        //public async Task<ActionResult<IEnumerable<UserDto>>> GetAllAdmins()
        //{
        //    var users = await _userService.GetAllAdminsAsync();
        //    if (users == null)
        //    {
        //        return NotFound("No admin found.");
        //    }
        //    return Ok(users);
        //}


        [HttpGet("admins-list")]
        public async Task<ActionResult<IEnumerable<AdminDto>>> GetAdminsForDropdown()
        {
            var admins = await _userService.GetAdminsForDropdownAsync();
            return Ok(admins);
        }


        [HttpGet("blacklisted/paginated")]
        public async Task<ActionResult<PaginatedResult<UserDto>>> GetPaginatedBlacklistedUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchQuery = null,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] string? sortOrder = "desc"
        )
        {
            var result = await _userService.GetPaginatedBlacklistedUsersAsync(page, pageSize, searchQuery, sortBy, sortOrder);
            return Ok(result);
        }


        //[HttpGet("blacklisted")]
        //public async Task<ActionResult<IEnumerable<UserDto>>> GetAllBlacklistedUsers()
        //{
        //    var users = await _userService.GetAllBlacklistedUsersAsync();
        //    if (users == null)
        //    {
        //        return NotFound("No blacklisted users found.");
        //    }
        //    return Ok(users);
        //}


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


        [HttpGet("{id}/details")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserDetails(Guid id)
        {
            // The controller now only calls the service
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


        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromForm] UpdateUserFormDto dto)
        {
            var userIdClaim = User.FindFirst("id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return Unauthorized();
            }

            var updatedUserDto = await _userService.UpdateCurrentUserAsync(userId, dto);

            if (updatedUserDto == null)
            {
                return NotFound("User not found or update failed.");
            }
            return Ok(updatedUserDto);
        }


        [Authorize]
        [HttpPut("me/name")]
        public async Task<IActionResult> UpdateCurrentUserName([FromBody] UpdateUserNameDto dto)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var updatedUserDto = await _userService.UpdateCurrentUserNameAsync(userId, dto);

            if (updatedUserDto == null)
            {
                return NotFound("User not found.");
            }

            return Ok(updatedUserDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UpdateUserFormDto dto)
        {
            var updatedUserDto = await _userService.UpdateUserAsync(id, dto);
            if (updatedUserDto == null)
            {
                return NotFound("User not found or update failed.");
            }
            return Ok(updatedUserDto);
        }


        //Actually, we don't delete users in our app. This is just for testing purposes.
        //We have to use gmail again and again cause of insufficient gamil account.
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok("Deleted successfully");
        }
    }
}
