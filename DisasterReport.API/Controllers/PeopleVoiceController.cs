using DisasterReport.Data.Domain;
using DisasterReport.Services.Models.PeopleVoiceDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DisasterReport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleVoiceController : ControllerBase
    {
        private readonly IPeopleVoiceService _peopleVoiceService;

        public PeopleVoiceController(IPeopleVoiceService peopleVoiceService)
        {
            _peopleVoiceService = peopleVoiceService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PeopleVoice>>> GetAllPeopleVoices()
        {
            var peopleVoices = await _peopleVoiceService.GetAllPeopleVoicesAsync();
            return Ok(peopleVoices);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PeopleVoice>> GetPeopleVoiceById(int id)
        {
            var peopleVoice = await _peopleVoiceService.GetPeopleVoiceByIdAsync(id);
            if (peopleVoice == null)
            {
                return NotFound();
            }
            return Ok(peopleVoice);
        }

        [HttpPost]
        public async Task<ActionResult<PeopleVoice>> CreatePeopleVoice([FromBody] CreatePeopleVoiceDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdPeopleVoice = await _peopleVoiceService.CreatePeopleVoiceAsync(createDto);
                return CreatedAtAction(nameof(GetPeopleVoiceById), new { id = createdPeopleVoice.Id }, createdPeopleVoice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PeopleVoice>> UpdatePeopleVoice(int id, [FromBody] UpdatePeopleVoiceDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedPeopleVoice = await _peopleVoiceService.UpdatePeopleVoiceAsync(id, updateDto);
                if (updatedPeopleVoice == null)
                {
                    return NotFound();
                }
                return Ok(updatedPeopleVoice);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePeopleVoice(int id)
        {
            var result = await _peopleVoiceService.DeletePeopleVoiceAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }
    }
}
