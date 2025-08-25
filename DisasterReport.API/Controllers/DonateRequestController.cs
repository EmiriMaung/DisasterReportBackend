using DisasterReport.Services.Models;
using DisasterReport.Services.Services;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DisasterReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonateRequestController : ControllerBase
    {
        private readonly IDonateRequestService _service;
        public DonateRequestController(IDonateRequestService service)
        {
            _service = service;
        }
        [HttpPost]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult<DonateRequestReadDto>> Create(
            [FromForm] DonateRequestCreateDto dto,
            IFormFile? slipFile)
        {
            // Get current user's ID from JWT
            var userId = GetUserId();

            var result = await _service.CreateAsync(dto, slipFile, userId);
            return Ok(result);
        }

        // ✅ Admin/org fetch all requests
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var requests = await _service.GetAllAsync();
            return Ok(requests);
        }
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _service.ApproveAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost("{id}/reject")]
        public async Task<IActionResult> Reject(int id)
        {
            var result = await _service.RejectAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // ✅ New endpoint to get by platform donation
        [Authorize(Roles = "Admin")]
        [HttpGet("platform/{isPlatformDonation}")]
        public async Task<ActionResult<IEnumerable<DonateRequestReadDto>>> GetByIsPlatform(bool isPlatformDonation)
        {
            var result = await _service.GetByIsPlatformAsync(isPlatformDonation);

            if (result == null || !result.Any())
                return NotFound("No donate requests found for the given type.");

            return Ok(result);
        }

        // ✅ Org fetch only pending requests
        [HttpGet("organization/{orgId}/pending")]
        public async Task<ActionResult<IEnumerable<DonateRequestReadDto>>> GetPendingByOrganization(int orgId)
        {
            var result = await _service.GetPendingByOrganizationIdAsync(orgId);

            if (result == null || !result.Any())
                return NotFound("No pending donate requests found for this organization.");

            return Ok(result);
        }


        // Utility to extract user ID from JWT
        private Guid GetUserId()
        {
            var userIdStr = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            return Guid.Parse(userIdStr!);
        }

    }
}
