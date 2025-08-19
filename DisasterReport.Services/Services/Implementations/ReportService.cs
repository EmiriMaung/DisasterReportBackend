using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Enums;
using DisasterReport.Services.Models.Common;
using DisasterReport.Services.Models.ReportDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

public class ReportService : IReportService
{
    private readonly IReportRepo _reportRepo;
    private readonly IUserRepo _userRepo;
    private readonly IPostRepo _postRepo;
    private readonly IBlacklistEntryRepo _blacklistEntryRepo;
    private readonly IMemoryCache _cache;
    private readonly CancellationTokenSource _cacheResetTokenSource = new();

    public ReportService(IReportRepo reportRepo, IUserRepo userRepo, IMemoryCache cache)
    {
        _reportRepo = reportRepo;
        _userRepo = userRepo;
        _cache = cache;
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
        Guid? adminId
    )
    {
        if (page <= 0) page = 1;
        if (pageSize <= 0) pageSize = 10;

        string cacheKey = $"Reports_{page}_{pageSize}_{searchQuery}_{sortBy}_{sortOrder}_{statusFilter}_{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}_{adminId}";

        if (_cache.TryGetValue(cacheKey, out PaginatedResult<ReportDto> cachedResult))
        {
            return cachedResult;
        }

        var (reports, totalCount) = await _reportRepo.GetAllAsync(
            page, pageSize, searchQuery, sortBy, sortOrder, statusFilter, startDate, endDate, adminId
        );

        // Collect admin IDs for name lookup
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
            CreatedAt = r.CreatedAt,
            ReviewedById = r.ReviewedBy,
            ReviewedByName = r.ReviewedBy.HasValue && adminNames.TryGetValue(r.ReviewedBy.Value, out var name) ? name : null,
            ReviewedAt = r.ReviewedAt
        }).ToList();

        var result = new PaginatedResult<ReportDto>
        {
            Items = reportDtos,
            TotalItems = totalCount,
            Page = page,
            PageSize = pageSize
        };

        var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
            .AddExpirationToken(new CancellationChangeToken(_cacheResetTokenSource.Token));

        _cache.Set(cacheKey, result, cacheEntryOptions);

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
            CreatedAt = report.CreatedAt,
            ReviewedById = report.ReviewedBy,
            ReviewedByName = reviewerName,
            ReviewedAt = report.ReviewedAt
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

        return new ReportDto
        {
            Id = created.Id,
            ReporterId = created.ReporterId,
            //ReporterName = created.Reporter.Name,
            ReportedUserId = created.ReportedUserId,
            //ReportedUserName = created.ReportedUser?.Name,
            ReportedPostId = created.ReportedPostId,
            Reason = created.Reason,
            Status = created.Status,
            CreatedAt = created.CreatedAt
        };
    }


    public async Task<ReportDto?> ResolveReportAsync(int id, Guid adminId, string actionTaken)
    {
        var resolved = await _reportRepo.ResolveAsync(id, adminId, actionTaken);
        if (resolved == null) return null;

        return new ReportDto
        {
            Id = resolved.Id,
            ReporterId = resolved.ReporterId,
            //ReporterName = resolved.Reporter.Name,
            ReportedUserId = resolved.ReportedUserId,
            //ReportedUserName = resolved.ReportedUser?.Name,
            ReportedPostId = resolved.ReportedPostId,
            Reason = resolved.Reason,
            Status = resolved.Status,
            ActionTaken = resolved.ActionTaken,
            CreatedAt = resolved.CreatedAt,
            ReviewedById = resolved.ReviewedBy,
            ReviewedByName = resolved.ReviewedBy.HasValue ? resolved.ReviewedBy.ToString() : null,
            ReviewedAt = resolved.ReviewedAt
        };
    }


    public async Task<ReportDto?> RejectReportAsync(int id, Guid adminId)
    {
        var rejected = await _reportRepo.RejectAsync(id, adminId);
        if (rejected == null) return null;

        return new ReportDto
        {
            Id = rejected.Id,
            ReporterId = rejected.ReporterId,
            //ReporterName = rejected.Reporter.Name,
            ReportedUserId = rejected.ReportedUserId,
            //ReportedUserName = rejected.ReportedUser?.Name,
            ReportedPostId = rejected.ReportedPostId,
            Reason = rejected.Reason,
            Status = rejected.Status,
            ActionTaken = rejected.ActionTaken,
            CreatedAt = rejected.CreatedAt,
            ReviewedById = rejected.ReviewedBy,
            ReviewedByName = rejected.ReviewedBy.HasValue ? rejected.ReviewedBy.ToString() : null,
            ReviewedAt = rejected.ReviewedAt
        };
    }
}
