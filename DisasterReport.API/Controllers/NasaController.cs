using DisasterReport.Services.Services.Implementations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DisasterReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NasaController : ControllerBase
    {
        private readonly INasaService _nasaService;
        private readonly IUsgsService _usgsService;

        public NasaController(INasaService nasaService, IUsgsService usgsService)
        {
            _nasaService = nasaService;
            _usgsService = usgsService;
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetEvents()
        {
            var eventsData = await _nasaService.GetAllDisasterEventsAsync();
            return Ok(eventsData);
        }
        [HttpGet("usgs-event")]
        public async Task<IActionResult> GetUsgsEvent()
        {
            var eventsData = await _usgsService.GetEarthquakeEventsAsync();
            return Ok(eventsData);
        }
    }
}