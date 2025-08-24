using DisasterReport.Services.Enums;
using DisasterReport.Services.Models.ReportDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("api/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    [HttpPost]
    [Authorize] // Any authenticated user
    public async Task<IActionResult> CreateReport([FromBody] ReportCreateDto dto)
    {
        // Optionally, you can set ReporterId from the JWT claims
        // dto.ReporterId = Guid.Parse(User.FindFirst("id")?.Value ?? throw new Exception("User Id not found"));

        var createdReport = await _reportService.CreateReportAsync(dto);
        return CreatedAtAction(nameof(GetReportById), new { id = createdReport.Id }, createdReport);
    }


    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllReports(
        int page = 1,
        int pageSize = 10,
        string? searchQuery = null,
        string? sortBy = null,
        string? sortOrder = null,
        string? statusFilter = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        Guid? adminId = null,
        string? reportFilterType = null
    )
    {
        var reports = await _reportService.GetAllReportsAsync(
            page, pageSize, searchQuery, sortBy, sortOrder, statusFilter, startDate, endDate, adminId, reportFilterType
        );
        return Ok(reports);
    }


    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReportById(int id)
    {
        var report = await _reportService.GetReportByIdAsync(id);
        if (report == null) return NotFound();
        return Ok(report);
    }


    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReportStats()
    {
        var stats = await _reportService.GetReportStatsAsync();
        return Ok(stats);
    }


    [HttpPost("{id}/resolve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ResolveReport(int id, [FromBody] ResolveReportDto dto)
    {
        Guid adminId = Guid.Parse(User.FindFirst("id")!.Value);

        var resolved = await _reportService.ResolveReportAsync(id, adminId, dto.ActionTaken);
        if (resolved == null) return BadRequest("Report not found or already handled.");
        return Ok(resolved);
    }


    //[HttpPost("{id}/reject")]
    //[Authorize(Roles = "Admin")]
    //public async Task<IActionResult> RejectReport(int id)
    //{
    //    Guid adminId = Guid.Parse(User.FindFirst("id")!.Value);
    //    var rejected = await _reportService.RejectReportAsync(id, adminId);
    //    if (rejected == null) return BadRequest("Report not found or already handled.");
    //    return Ok(rejected);
    //}
    [HttpPost("{id}/reject")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RejectReport(int id)
    {
        try
        {
            Guid adminId = Guid.Parse(User.FindFirst("id")!.Value);
            var rejected = await _reportService.RejectReportAsync(id, adminId);
            if (rejected == null) return BadRequest("Report not found or already handled.");
            return Ok(rejected);
        }
        catch (Exception ex)
        {
            // Log the actual error
            return StatusCode(500, new { message = ex.Message, stackTrace = ex.StackTrace });
        }
    }
}
