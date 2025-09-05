using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DisasterReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var stats = await _dashboardService.GetDashboardStatsAsync();
            return Ok(stats);
        }
        [HttpGet("platform-donations-last-30-days")]
        public async Task<IActionResult> GetPlatformDonationsLast30Days()
        {
            var result = await _dashboardService.GetLast30DaysPlatformDonationsAsync();
            return Ok(result);
        }
       
    }

}
