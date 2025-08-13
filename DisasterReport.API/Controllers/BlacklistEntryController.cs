using ClosedXML.Excel;
using DisasterReport.Services.Models.BlacklistEntryDTO;
using DisasterReport.Services.Models.Common;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using System.ComponentModel;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using ClosedXML.Excel;

namespace DisasterReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlacklistEntryController : ControllerBase
    {
        private readonly IBlacklistEntryService _blacklistEntryService;
        public BlacklistEntryController(IBlacklistEntryService blacklistEntryService)
        {
            _blacklistEntryService = blacklistEntryService;
        }


        [HttpGet("paginated/all")]
        public async Task<ActionResult<PaginatedResult<BlacklistEntryDto>>> GetAllBlacklistEntriesAsync(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchQuery = null,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] string? sortOrder = "desc",
            [FromQuery] string? statusFilter = "all",
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null,
            [FromQuery] Guid? adminId = null
        )
        {
            var result = await _blacklistEntryService.GetAllBlacklistEntriesAsync(page, pageSize, searchQuery, sortBy, sortOrder, statusFilter, startDate, endDate, adminId);
            return Ok(result);
        }


        [HttpGet("blacklist-stats")]
        public async Task<ActionResult<BlacklistStatsDto>> GetBlacklistStats()
        {
            var stats = await _blacklistEntryService.GetBlacklistStatsAsync();
            return Ok(stats);
        }


        [HttpGet("blacklist-detail/{id}")]
        public async Task<ActionResult<BlacklistDetailDto>> GetBlacklistDetailsById(int id)
        {
            try
            {
                var entry = await _blacklistEntryService.GetBlacklistDetailByIdAsync(id);
                return Ok(entry);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<BlacklistEntryDto>> GetById(int id)
        {
            try
            {
                var entry = await _blacklistEntryService.GetBlacklistEntryByIdAsync(id);
                return Ok(entry);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        //[HttpGet("user-history/{userId}")]
        //public async Task<ActionResult<IEnumerable<BlacklistHistoryDto>>> GetUserHistory(Guid userId)
        //{
        //    var history = await _blacklistEntryService.GetUserBlacklistHistoryAsync(userId);
        //    return Ok(history);
        //}


        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportBlacklist()
        {
            var data = await _blacklistEntryService.GetAllBlacklistForExportAsync();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Blacklist History");

                // Add headers
                worksheet.Cell(1, 1).Value = "Name";
                worksheet.Cell(1, 2).Value = "Email";
                worksheet.Cell(1, 3).Value = "Reason";
                worksheet.Cell(1, 4).Value = "CreatedAdminName";
                worksheet.Cell(1, 5).Value = "CreatedAt";
                worksheet.Cell(1, 6).Value = "UnblockedReason";
                worksheet.Cell(1, 7).Value = "UnblockedAdminName";
                worksheet.Cell(1, 8).Value = "UnblockedAt";

                // Style headers
                var headerRow = worksheet.Range(1, 1, 1, 8);
                headerRow.Style.Font.Bold = true;

                // Add data
                int row = 2;
                foreach (var item in data)
                {
                    worksheet.Cell(row, 1).Value = item.Name;
                    worksheet.Cell(row, 2).Value = item.Email;
                    worksheet.Cell(row, 3).Value = item.Reason;
                    worksheet.Cell(row, 4).Value = item.CreatedAdminName;
                    worksheet.Cell(row, 5).Value = item.CreatedAt;
                    worksheet.Cell(row, 6).Value = item.UpdatedReason;
                    worksheet.Cell(row, 7).Value = item.UpdatedAdminName;
                    worksheet.Cell(row, 8).Value = item.UpdatedAt;
                    row++;
                }

                worksheet.ColumnsUsed().AdjustToContents();

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var bytes = stream.ToArray();

                    // Set proper response headers
                    Response.Headers.Add("Content-Disposition", "attachment; filename=\"blacklist_export.xlsx\"");

                    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "blacklist_export.xlsx");
                }
            }
        }


        [HttpGet("export-pdf")]
        public async Task<IActionResult> ExportBlacklistPdf()
        {
            var data = await _blacklistEntryService.GetAllBlacklistForExportAsync();

            var document = new BlacklistDocument(data);
            var bytes = document.GeneratePdf();

            return File(bytes, "application/pdf", "blacklist_export.pdf");
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBlacklistEntryDto dto)
        {
            try
            {
                await _blacklistEntryService.AddAsync(dto);
                return StatusCode(201);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReason(int id, [FromBody] UnblockUserDto dto)
        {
            try
            {
                await _blacklistEntryService.UpdateReasonAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpDelete("unblock/{userId}")]
        public async Task<IActionResult> UnbanUser(Guid userId, [FromBody] UnblockUserDto dto)
        {
            try
            {
                var adminIdClaim = User.FindFirst("id") ?? User.FindFirst(ClaimTypes.NameIdentifier);
                if (adminIdClaim == null || !Guid.TryParse(adminIdClaim.Value, out var adminId))
                {
                    return Unauthorized(); 
                }

                await _blacklistEntryService.SoftDeleteAsync(userId, adminId, dto.UnblockedReason);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpGet("check/{userId}")]
        public async Task<ActionResult<bool>> IsUserBlacklisted(Guid userId)
        {
            var isBlacklisted = await _blacklistEntryService.IsUserBlacklistedAsync(userId);
            return Ok(isBlacklisted);
        }
    }
}
