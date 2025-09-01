using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Implementations;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DisasterReport.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _service;

        public ActivitiesController(IActivityService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var activities = await _service.GetAllActivitiesAsync();
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var activity = await _service.GetActivityByIdAsync(id);
            if (activity == null) return NotFound();
            return Ok(activity);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ActivityDto dto)
        {
            var activity = await _service.CreateActivityAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = activity.Id }, activity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ActivityDto dto)
        {
            var updated = await _service.UpdateActivityAsync(id, dto);
            if (!updated) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteActivityAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

        [HttpPost("{id}/media")]
        public async Task<IActionResult> AddMedia(int id, ActivityMediumDto dto)
        {
            dto.ActivityId = id;
            var media = await _service.AddMediaAsync(dto);
            return Ok(media);
        }

        [HttpDelete("media/{id}")]
        public async Task<IActionResult> DeleteMedia(int id)
        {
            var deleted = await _service.DeleteMediaAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }

    }
}
