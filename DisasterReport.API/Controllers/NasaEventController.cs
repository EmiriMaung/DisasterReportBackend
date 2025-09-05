using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DisasterReport.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NasaEventsController : ControllerBase
    {
        private readonly IDisasterEventNasaService _service;

        public NasaEventsController(IDisasterEventNasaService service)
        {
            _service = service;
        }

        [HttpPost("fetch")]
        public async Task<IActionResult> FetchAndSave()
        {
            await _service.FetchAndStoreDisastersAsync();
            return Ok();
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _service.GetDisasterEventsAsync();
            return Ok(events);
        }
    }

}
