using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DisasterReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisastersReportController : ControllerBase
    {
        private readonly IDisasterReportService _disasterReportService;

        public DisastersReportController(IDisasterReportService disasterReportService)
        {
            _disasterReportService = disasterReportService ?? throw new ArgumentNullException(nameof(disasterReportService));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DisasterReportDto>>> GetAllReportsAsync()
        {
            var reports = await _disasterReportService.GetAllReportsAsync();
            return Ok(reports);
        }
        [HttpGet("urgent")]
        public async Task<ActionResult<IEnumerable<DisasterReportDto>>> GetUrgentReportsAsync()
        {
            var reports = await _disasterReportService.GetUrgentReportsAsync();
            return Ok(reports);
        }
        [HttpGet("reporter/{reporterId}")]
        public async Task<ActionResult<IEnumerable<DisasterReportDto>>> GetAllReportsByReporterIdAsync(Guid reporterId)
        {
            var reports = await _disasterReportService.GetAllReportsByReporterIdAsync(reporterId);
            return Ok(reports);
        }
        [HttpGet("reporter/{reporterId}/deleted-report")]
        public async Task<ActionResult<IEnumerable<DisasterReportDto>>> GetDeletedReportsByReporterIdAsync(Guid reporterId)
        {
            var reports = await _disasterReportService.GetDeletedReportsByReporterIdAsync(reporterId);
            return Ok(reports);
        }

        [HttpGet("reports/{id}")]
        public async Task<ActionResult<DisasterReportDto>> GetReportByIdAsync(int id)
        {
            var report = await _disasterReportService.GetReportByIdAsync(id);
            if (report == null)
                return NotFound("Report not found.");

            return Ok(report);
        }
        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<DisasterReportDto>>> GetReportsByStatusAsync(int status)
        {
            var reports = await _disasterReportService.GetReportsByStatusAsync(status);
            return Ok(reports);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<DisasterReportDto>>> SearchReportsAsync(
            [FromQuery] string? keyword,
            [FromQuery] string? category,
            [FromQuery] string? region,
            [FromQuery] bool? isUrgent)
        {
            var reports = await _disasterReportService.SearchReportsAsync(keyword, category, region, isUrgent);
            return Ok(reports);
        }

        [HttpGet("region/{regionName}")]
        public async Task<ActionResult<IEnumerable<DisasterReportDto>>> GetReportsByRegionAsync(string regionName)
        {
            var reports = await _disasterReportService.GetReportsByRegionAsync(regionName);
            return Ok(reports);
        }

        [HttpGet("township/{townshipName}")]
        public async Task<ActionResult<IEnumerable<DisasterReportDto>>> GetReportsByTownshipAsync(string townshipName)
        {
            var reports = await _disasterReportService.GetReportsByTownshipAsync(townshipName);
            return Ok(reports);
        }
        [HttpPost("add-disaster-report")]
        public async Task<IActionResult> AddReportAsync([FromForm] AddDisasterReportDto report)
        {
            if (report == null)
                return BadRequest("Report data is required.");

            await _disasterReportService.AddReportAsync(report);
            return Ok("Report added successfully.");
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReportAsync(int id, [FromForm] UpdateDisasterReportDto dto)
        {
            await _disasterReportService.UpdateReportAsync(id, dto);
            return Ok("Report updated successfully.");
        }


        [HttpDelete("soft-delete/{id}")]
        public async Task<IActionResult> SoftDeleteAsync(int id)
        {
            await _disasterReportService.SoftDeleteAsync(id);
            return Ok("SoftDeleted successfully.");
        }

        [HttpPatch("restore-deletedreport/{id}")]
        public async Task<IActionResult> RestoreAsync(int id)
        {
            await _disasterReportService.RestoreReportAsync(id);
            return Ok("restore successfully.");
        }
        [HttpDelete("{id}/permanent")]
        public async Task<IActionResult> HardDeleteAsync(int id)
        {
            await _disasterReportService.HardDeleteAsync(id);
            return NoContent();
        }
        [HttpPost("{reportId}/approve")]
        public async Task<IActionResult> ApproveReportAsync(int reportId, [FromQuery] Guid approvedBy)
        {
            await _disasterReportService.ApproveReportAsync(reportId, approvedBy);
            return Ok(new { message = "Report approved successfully." });
        }

        [HttpPost("{reportId}/reject")]
        public async Task<IActionResult> RejectReportAsync(int reportId, [FromQuery] Guid rejectedBy)
        {
            await _disasterReportService.RejectReportAsync(reportId, rejectedBy);
            return Ok(new { message = "Report rejected successfully." });
        }

    }
}
