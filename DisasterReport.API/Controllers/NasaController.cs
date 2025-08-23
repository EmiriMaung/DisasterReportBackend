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

        public NasaController(INasaService nasaService)
        {
            _nasaService = nasaService;
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetEvents()
        {
            var eventsData = await _nasaService.GetAllDisasterEventsAsync();
            return Ok(eventsData);
        }
    }
}