using DisasterReport.Services.Models.BlacklistEntryDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            await _blacklistEntryService.AddAsync(dto);
            return StatusCode(201);
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


        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id, [FromQuery] Guid adminId)
        {
            await _blacklistEntryService.SoftDeleteAsync(id, adminId);
            return NoContent();
        }


        [HttpGet("check/{userId}")]
        public async Task<ActionResult<bool>> IsUserBlacklisted(Guid userId)
        {
            var isBlacklisted = await _blacklistEntryService.IsUserBlacklistedAsync(userId);
            return Ok(isBlacklisted);
        }
    }
}
