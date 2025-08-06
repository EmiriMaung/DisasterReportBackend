using DisasterReport.Services.Models.BlacklistEntryDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DisasterReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlacklistEntryController : ControllerBase
    {
        private readonly IBlacklistEntryService _blacklistEntryService;
        public BlacklistEntryController(IBlacklistEntryService blacklistEntryService)
        {
            _blacklistEntryService = blacklistEntryService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlacklistEntryDto>>> GetAll()
        {
            var entries = await _blacklistEntryService.GetAllBlacklistEntriesAsync();
            return Ok(entries);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<BlacklistEntryDto>> GetById(int id)
        {
            try
            {
                var entry = await _blacklistEntryService.GetBlacklistEntryByIdAsync(id);
                return Ok(entry);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBlacklistEntryDto dto)
        {
            try
            {
                await _blacklistEntryService.AddAsync(dto);
                return StatusCode(201);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReason(int id, [FromBody] UpdateBlacklistEntryDto dto)
        {
            try
            {
                await _blacklistEntryService.UpdateReasonAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpDelete("{userId}")]
        public async Task<IActionResult> UnbanUser(Guid userId)
        {
            try
            {
                var adminIdClaim = User.FindFirst("id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
                if (adminIdClaim == null || !Guid.TryParse(adminIdClaim.Value, out var adminId))
                {
                    return Unauthorized(); 
                }

                await _blacklistEntryService.SoftDeleteAsync(userId, adminId);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpGet("check/{userId}")]
        public async Task<ActionResult<bool>> IsUserBlacklisted(Guid userId)
        {
            var isBlacklisted = await _blacklistEntryService.IsUserBlacklistedAsync(userId);
            return Ok(isBlacklisted);
        }
    }
}
