using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Models.Common;
using DisasterReport.Services.Models.ReportDTO;
using DisasterReport.Services.Services.Implementations;
using DisasterReport.Services.Services.Interfaces;
using DisasterReport.Shared.SignalR;

public class ReportService : IReportService
{
    private readonly IReportRepo _reportRepo;
    private readonly IUserRepo _userRepo;

    public ReportService(IReportRepo reportRepo, IUserRepo userRepo)
    {
        _reportRepo = reportRepo;
        _userRepo = userRepo;
    }

    public async Task<PaginatedResult<ReportDto>> GetAllReportsAsync(
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
    )
    {
        var (reports, totalCount) = await _reportRepo.GetAllAsync(
            page, pageSize, searchQuery, sortBy, sortOrder, statusFilter, startDate, endDate, adminId, reportFilterType
        );

        var adminIds = reports
            .Where(r => r.ReviewedBy.HasValue)
            .Select(r => r.ReviewedBy!.Value)
            .Distinct()
            .ToList();

        var adminNames = await _userRepo.GetUserNamesByIdsAsync(adminIds);

        var reportDtos = reports.Select(r => new ReportDto
        {
            Id = r.Id,
            ReporterId = r.ReporterId,
            ReporterName = r.Reporter.Name,
            ReportedUserId = r.ReportedUserId,
            ReportedUserName = r.ReportedUser?.Name,
            ReportedPostId = r.ReportedPostId,
            Reason = r.Reason,
            Status = r.Status,
            ActionTaken = r.ActionTaken,
            CreatedAt = DateTime.UtcNow,
            ReviewedById = r.ReviewedBy,
            ReviewedByName = r.ReviewedBy.HasValue && adminNames.TryGetValue(r.ReviewedBy.Value, out var name) ? name : null,
            ReviewedAt = DateTime.UtcNow
        }).ToList();

        var result = new PaginatedResult<ReportDto>
        {
            Items = reportDtos,
            TotalItems = totalCount,
            Page = page,
            PageSize = pageSize
        };

        return result;
    }


    public async Task<ReportDto?> GetReportByIdAsync(int id)
    {
        var report = await _reportRepo.GetByIdAsync(id);
        if (report == null) return null;

        string? reviewerName = null;
        if (report.ReviewedBy.HasValue)
        {
            var names = await _userRepo.GetUserNamesByIdsAsync(new List<Guid> { report.ReviewedBy.Value });
            reviewerName = names.TryGetValue(report.ReviewedBy.Value, out var name) ? name : null;
        }

        return new ReportDto
        {
            Id = report.Id,
            ReporterId = report.ReporterId,
            ReporterName = report.Reporter.Name,
            ReportedUserId = report.ReportedUserId,
            ReportedUserName = report.ReportedUser?.Name,
            ReportedPostId = report.ReportedPostId,
            Reason = report.Reason,
            Status = report.Status,
            ActionTaken = report.ActionTaken,
            CreatedAt = DateTime.UtcNow,
            ReviewedById = report.ReviewedBy,
            ReviewedByName = reviewerName,
            ReviewedAt = DateTime.UtcNow
        };
    }


    public async Task<ReportStatsDto> GetReportStatsAsync()
    {
        var statsCounts = await _reportRepo.GetStatsAsync();

        var statsDto = new ReportStatsDto
        {
            Total = statsCounts.GetValueOrDefault("Total"),
            PendingReports = statsCounts.GetValueOrDefault("PendingReports"),
            Resolved = statsCounts.GetValueOrDefault("Resolved"),
            Reject = statsCounts.GetValueOrDefault("Reject")
        };

        return statsDto;
    }


    public async Task<ReportDto> CreateReportAsync(ReportCreateDto dto)
    {
        var report = new Report
        {
            ReporterId = dto.ReporterId,
            ReportedPostId = dto.ReportedPostId,
            ReportedUserId = dto.ReportedUserId,
            Reason = dto.Reason,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        var created = await _reportRepo.CreateAsync(report);

        return new ReportDto { };
    }


    public async Task<ReportDto?> ResolveReportAsync(int id, Guid adminId, string actionTaken)
    {
        var resolved = await _reportRepo.ResolveAsync(id, adminId, actionTaken);
        if (resolved == null) return null;

        return new ReportDto { };
    }

    public async Task<ReportDto?> RejectReportAsync(int id, Guid adminId)
    {
        var rejected = await _reportRepo.RejectAsync(id, adminId);
        if (rejected == null) return null;

        return new ReportDto { };
    }
}