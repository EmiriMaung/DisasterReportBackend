using DisasterReport.Data.Domain;
using DisasterReport.Services.Models;
using DisasterReport.Services.Models.Common;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DisasterReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
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
       
        [HttpGet("my-reports")]
        public async Task<ActionResult<IEnumerable<DisasterReportDto>>> GetMyReportsAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid reporterId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            var reports = await _disasterReportService.GetMyReportsAsync(reporterId);
            return Ok(reports);
        }

        [HttpGet("my-deleted-reports")]
        public async Task<ActionResult<IEnumerable<DisasterReportDto>>> GetMyDeletedReportsAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid reporterId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            var reports = await _disasterReportService.GetMyDeletedReportsAsync(reporterId);
            return Ok(reports);
        }
        [HttpGet("reporter/{reporterId}")]
        public async Task<IActionResult> GetAllReportsByReporter(Guid reporterId)
        {
            var reports = await _disasterReportService.GetAllReportsByReporterIdAsync(reporterId);
            return Ok(reports);
        }

        // GET: api/ReporterReports/reporter/{reporterId}/deleted
        [HttpGet("reporter/{reporterId}/deleted")]
        public async Task<IActionResult> GetDeletedReportsByReporter(Guid reporterId)
        {
            var deletedReports = await _disasterReportService.GetDeletedReportsByReporterIdAsync(reporterId);
            return Ok(deletedReports);
        }
        [HttpGet("reports/{id}")]
        public async Task<ActionResult<DisasterReportDto>> GetReportByIdAsync(int id)
        {
            var report = await _disasterReportService.GetReportByIdAsync(id);
            if (report == null)
                return NotFound("Report not found.");

            return Ok(report);
        }

        [HttpGet("countreportbystatus")]
        public async Task<ActionResult<ReportStatusCountDto>> CountReportByStatus()
        {
            var result = await _disasterReportService.CountReportsByStatusAsync();
            return Ok(result);
        }

        [HttpGet("status")]
        public async Task<ActionResult<PagedResponse<DisasterReportDto>>> GetReportsByStatusAsync(
        [FromQuery] int? status,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 18)
        {
            var reports = await _disasterReportService.GetReportsByStatusAsync(status, pageNumber, pageSize);
            return Ok(reports);
        }
        [HttpGet("topic/{topicId}")]
        public async Task<IActionResult> GetReportsByTopic(int topicId)
        {
            var reports = await _disasterReportService.GetReportsByTopicIdAsync(topicId);
            return Ok(reports);
        }
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<DisasterReportDto>>> SearchReportsAsync(
            [FromQuery] string? keyword,
            [FromQuery] string? category,
            [FromQuery] int? topicId,
            [FromQuery] string? township,
            [FromQuery] string? region,
            [FromQuery] bool? isUrgent)

        {
            var reports = await _disasterReportService.SearchReportsAsync(keyword, category, region,township, isUrgent,topicId);
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

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid reporterId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            try
            {
                await _disasterReportService.AddReportAsync(report, reporterId);
                return Ok("Report added successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while adding the report");
            }
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
        public async Task<IActionResult> ApproveReportAsync(int reportId, [FromBody] ApproveWithTopicDto topicDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid approvedBy))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            try
            {
                if (topicDto.NewTopic != null)
                {
                    topicDto.NewTopic.AdminId = approvedBy;
                }

                await _disasterReportService.ApproveReportAsync(reportId, topicDto);
                return Ok(new { message = "Report approved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to approve report", error = ex.Message });
            }
        }

        [HttpPost("{reportId}/reject")]
        public async Task<IActionResult> RejectReportAsync(int reportId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid rejectedBy))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            try
            {
                await _disasterReportService.RejectReportAsync(reportId, rejectedBy);
                return Ok(new { message = "Report rejected successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to reject report", error = ex.Message });
            }
        }
        [HttpGet("{reportId}/related")]
        public async Task<IActionResult> GetRelatedReportsAsync(int reportId)
        {
            var relatedReports = await _disasterReportService.GetRelatedReportsByTopicAsync(reportId);

            return Ok(relatedReports);
        }
        [HttpGet("map-reports")]
        public async Task<IActionResult> GetDisasterReportsForMap([FromQuery] ReportFilterDto filter)
        
        {
            
            
            var result = await _disasterReportService.GetDisasterReportsForMapAsync(filter);
            return Ok(result);
        }


    }
}
