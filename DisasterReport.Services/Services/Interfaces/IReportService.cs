using DisasterReport.Data.Domain;
using DisasterReport.Services.Enums;
using DisasterReport.Services.Models.Common;
using DisasterReport.Services.Models.ReportDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IReportService
    {
        Task<PaginatedResult<ReportDto>> GetAllReportsAsync(
        int page,
        int pageSize,
        string? searchQuery,
        string? sortBy,
        string? sortOrder,
        string? statusFilter,
        DateTime? startDate,
        DateTime? endDate,
        Guid? adminId,
        string? reportFilterType
    );

        Task<ReportDto?> GetReportByIdAsync(int id);

        Task<ReportStatsDto> GetReportStatsAsync();

        Task<ReportDto> CreateReportAsync(ReportCreateDto dto);

        Task<ReportDto?> ResolveReportAsync(int id, Guid adminId, string actionTaken);

        Task<ReportDto?> RejectReportAsync(int id, Guid adminId);
    }
}
