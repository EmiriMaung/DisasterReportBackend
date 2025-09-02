using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DisasterReport.Data.Dtos;
using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DisasterReport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActivityDto>>> GetAllActivities()
        {
            var activities = await _activityService.GetAllActivitiesAsync();
            return Ok(activities);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> GetActivity(int id)
        {
            var activity = await _activityService.GetActivityByIdAsync(id);
            if (activity == null) return NotFound();
            return Ok(activity);
        }

        [HttpPost]
        public async Task<ActionResult<ActivityDto>> CreateActivity([FromForm] CreateActivityDto createDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var activity = await _activityService.CreateActivityAsync(createDto);
            return CreatedAtAction(nameof(GetActivity), new { id = activity.Id }, activity);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ActivityDto>> UpdateActivity(int id, [FromBody] ActivityDto updateDto)
        {
            if (id != updateDto.Id) return BadRequest("ID mismatch");

            var activity = await _activityService.UpdateActivityAsync(id, updateDto);
            if (activity == null) return NotFound();
            return Ok(activity);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteActivity(int id)
        {
            var result = await _activityService.DeleteActivityAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }

        [HttpPost("{activityId}/media")]
        public async Task<ActionResult<ActivityMediumDto>> AddMedia(int activityId, IFormFile mediaFile)
        {
            if (mediaFile == null || mediaFile.Length == 0)
                return BadRequest("No file provided");

            var media = await _activityService.AddMediaToActivityAsync(activityId, mediaFile);
            if (media == null) return NotFound("Activity not found");
            return Ok(media);
        }

        [HttpDelete("media/{mediaId}")]
        public async Task<ActionResult> RemoveMedia(int mediaId)
        {
            var result = await _activityService.RemoveMediaFromActivityAsync(mediaId);
            if (!result) return NotFound();
            return NoContent();
        }

    }
}
