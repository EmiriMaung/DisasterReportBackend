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
            var userId = Guid.Parse(User.FindFirst("sub")?.Value ?? throw new Exception("UserId missing"));

            var result = await _service.CreateAsync(dto, slipFile);
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

    }
}
