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
        //[HttpGet("platform-donations-last-30-days")]
        //public IActionResult GetPlatformDonationsLast30Days()
        //{
        //    // Generate mock data for the last 30 days
        //    var today = DateTime.UtcNow.Date;
        //    var random = new Random();

        //    var donations = Enumerable.Range(0, 30).Select(i => new
        //    {
        //        donationDate = today.AddDays(-i),  // Last 30 days
        //        totalAmount = random.Next(1000, 100000) // Random amount between 1k and 10k
        //    })
        //    .OrderBy(d => d.donationDate) // Sort ascending (oldest → newest)
        //    .ToList();

        //    return Ok(donations);
        //}

    }

}
