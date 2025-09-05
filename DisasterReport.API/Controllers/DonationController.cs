using DisasterReport.Services.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DisasterReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationController : ControllerBase
    {
        private readonly IDonationService _donationService;

        public DonationController(IDonationService donationService)
        {
            _donationService = donationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var donations = await _donationService.GetAllAsync();
            return Ok(donations);
        }

        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetByUser(Guid userId)
        {
            var donations = await _donationService.GetByUserIdAsync(userId);
            return Ok(donations);
        }

        [HttpGet("organization/{orgId}")]
        [Authorize]
        public async Task<IActionResult> GetByOrganization(int orgId)
        {
            var donations = await _donationService.GetByOrganizationIdAsync(orgId);
            return Ok(donations);
        }

        [HttpGet("total")]
        public async Task<IActionResult> GetTotalAmount()
        {
            var total = await _donationService.GetTotalDonatedAmountAsync();
            return Ok(new { total });
        }

        [HttpGet("organization-summary")]
        public async Task<IActionResult> GetOrganizationDonationSummary()
        {
            var summary = await _donationService.GetOrganizationDonationSummaryAsync();

            if (summary == null || !summary.Any())
                return NotFound("No donations found for any organization.");

            return Ok(summary);
        }
    }
}
