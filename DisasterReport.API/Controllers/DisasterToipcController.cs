using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

namespace DisasterReport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Require JWT Authentication
    public class DisasterTopicController : ControllerBase
    {
        private readonly IDisasterTopicService _service;

        public DisasterTopicController(IDisasterTopicService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var topics = await _service.GetAllAsync();
            return Ok(topics);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var topic = await _service.GetByIdAsync(id);
            if (topic == null) return NotFound();

            return Ok(topic);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateDisasterTopicDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Admin ID not found in token.");

            if (!Guid.TryParse(userId, out var adminId))
                return Unauthorized("Invalid Admin ID format.");

            dto.AdminId = adminId;

            var created = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDisasterTopicDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Mismatched ID.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Admin ID not found in token.");

            if (!Guid.TryParse(userId, out var updatedAdminId))
                return Unauthorized("Invalid Admin ID format.");

            dto.UpdatedAdminId = updatedAdminId;

            var result = await _service.UpdateAsync(dto);
            if (!result)
                return NotFound();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();

            return NoContent();
        }
    }
}
