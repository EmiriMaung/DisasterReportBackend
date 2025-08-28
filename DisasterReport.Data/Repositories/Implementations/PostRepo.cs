using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DisasterReport.Data.Repositories;

public class PostRepo : IPostRepo
{
    private readonly ApplicationDBContext _context;

    public PostRepo(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<List<DisastersReport>> GetAllPostsByReporterId(Guid reporterId)
    {
        return await _context.DisastersReports
            .Where(r => r.ReporterId == reporterId && !r.IsDeleted && r.Status == 1)
            .AsNoTracking()
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            //.Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
            .Include(r => r.Reporter)
            .ToListAsync();
    }
    public async Task<List<DisastersReport>> GetPendingRejectReportByReporterIdAsync(Guid reporterId)
    {
        return await _context.DisastersReports
        .Where(r => r.ReporterId == reporterId
         && !r.IsDeleted
         && (r.Status == 0 || r.Status == 2))
            .AsNoTracking()
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            //.Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
            .Include(r => r.Reporter)
            .ToListAsync();
    }

    public async Task<List<DisastersReport>> GetDeletedPostsByReporterId(Guid reporterId)
    {
        return await _context.DisastersReports
            .Where(r => r.ReporterId == reporterId && r.IsDeleted)
            .AsNoTracking()
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            //.Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
            .Include(r => r.Reporter)
            .ToListAsync();
    }

    public async Task<List<DisastersReport>> GetAllPostsWithMaterialsAsync()
    {
        return await _context.DisastersReports
            .Where(r => r.Status == 1 && r.IsDeleted==false)
            .AsNoTracking()
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            //.Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
            .Include(r => r.Reporter)
            .ToListAsync();
    }

    public async Task<List<DisastersReport>> GetPendingReportsAsync()
    {
        return await _context.DisastersReports.Where(r => r.Status == 0).Include(r => r.Location)
            .AsNoTracking()
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            //.Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.Reporter)
            .Include(r => r.SupportTypes).ToListAsync();
    }

    public async Task<List<DisastersReport>> GetUrgentReportsAsync()
    {
        return await _context.DisastersReports.Where(r => r.IsUrgent && !r.IsDeleted && r.Status == 1).Include(r => r.Location)
            .AsNoTracking()
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            //.Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.Reporter)
            .Include(r => r.SupportTypes).ToListAsync();
    }
    public async Task<(int total, int approve, int pending, int reject)> GetReportCountsByStatusAsync()
    {
        var total = await _context.DisastersReports.CountAsync(r => r.IsDeleted == false);
        var approve = await _context.DisastersReports.CountAsync(r => r.IsDeleted == false && r.Status == 0);
        var pending = await _context.DisastersReports.CountAsync(r => r.IsDeleted == false && r.Status == 1);
        var reject = await _context.DisastersReports.CountAsync(r => r.IsDeleted == false && r.Status == 2);

        return (total, approve, pending, reject);
    }

    public async Task<List<DisastersReport>> GetReportsByStatusAsync(int? status)
    {
        var query = _context.DisastersReports
            .Where(r => !r.IsDeleted)
            .AsNoTracking();

        // Include statements
        query = query
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            //.Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
            .Include(r => r.Reporter); // this line is safe now

        // Filter by status
        if (status.HasValue)
        {
            query = query.Where(r => r.Status == status.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<List<DisastersReport>> GetReportsByOrganizationIdAsync(int organizationId)
    {
        // First get all user IDs that belong to this organization
        var organizationUserIds = await _context.OrganizationMembers
            .Where(om => om.OrganizationId == organizationId && om.IsAccepted)
            .Select(om => om.UserId)
            .ToListAsync();

        if (!organizationUserIds.Any())
        {
            return new List<DisastersReport>();
        }

        // Then get reports from those users
        return await _context.DisastersReports
            .Where(r => !r.IsDeleted && r.Status == 1 && organizationUserIds.Contains(r.ReporterId))
            .AsNoTracking()
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
            .Include(r => r.Reporter)
            .ToListAsync();
    }


    public async Task SoftDeleteReportAsync(int reportId)
    {
        var report = await _context.DisastersReports.FindAsync(reportId);
        if (report != null)
        {
            report.IsDeleted = true;
            _context.DisastersReports.Update(report);
        }
    }

    public async Task RestoreDeletedReportAsync(int reportId)
    {
        var report = await _context.DisastersReports.FindAsync(reportId);
        if (report != null)
        {
            report.IsDeleted = false;
            _context.DisastersReports.Update(report);
        }
    }
    public async Task<DisastersReport?> GetPostByIdAsync(int id)
    {
        return await _context.DisastersReports
            .Include(r => r.Location)
            .Include(r => r.ImpactUrls)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
            .Include(r => r.Comments)
            .Include(r => r.Reporter)
            .Include(r=>r.DisasterTopics)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<DisastersReport>> SearchReportsAsync(string? keyword, string? category, string? region,string? township, bool? isUrgent,int? topicId)
    {
        var query = _context.DisastersReports
            .AsNoTracking()
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            //.Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
            .Include(r => r.Reporter) 
            .Include(r=> r.DisasterTopics)
            .Where(r => !r.IsDeleted && r.Status == 1) 
            .AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(r => r.Title.Contains(keyword) || r.Description.Contains(keyword));

        if (topicId.HasValue)
            query = query.Where(r => r.DisasterTopicsId == topicId.Value);

        if (!string.IsNullOrEmpty(category))
            query = query.Where(r => r.Category == category);

        if (!string.IsNullOrEmpty(region))
            query = query.Where(r => r.Location.RegionName == region);

        if (!string.IsNullOrEmpty(township))
            query = query.Where(r => r.Location.TownshipName == township);

        if (isUrgent.HasValue)
            query = query.Where(r => r.IsUrgent == isUrgent.Value);

        return await query.ToListAsync();
    }

    public async Task ApproveReportAsync(int reportId, Guid approvedBy)
    {
        var report = await _context.DisastersReports.FindAsync(reportId);
        if (report != null)
        {
            report.Status = 1;
            report.UpdatedAt = DateTime.Now;
            _context.DisastersReports.Update(report);
        }
    }

    public async Task RejectReportAsync(int reportId, Guid rejectedBy)
    {
        var report = await _context.DisastersReports.FindAsync(reportId);
        if (report != null)
        {
            report.Status = 2;
            report.UpdatedAt = DateTime.UtcNow;
            _context.DisastersReports.Update(report);
        }
    }


    public async Task<List<DisastersReport>> GetReportsByRegionAsync(string regionName)
    {
        return await _context.DisastersReports.Where(r => r.Location.RegionName == regionName && !r.IsDeleted && r.Status == 1)
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            //.Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.Reporter)
            .Include(r => r.SupportTypes).ToListAsync();
    }

    public async Task<List<DisastersReport>> GetReportsByTownshipAsync(string townshipName)
    {
        return await _context.DisastersReports.Where(r => r.Location.TownshipName == townshipName && !r.IsDeleted && r.Status == 1)
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            //.Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.Reporter)
            .Include(r => r.SupportTypes).ToListAsync();
    }
    public async Task<List<DisastersReport>> GetReportsByTopicIdAsync(int topicId)
    {
        return await _context.DisastersReports
            .Where(r => r.DisasterTopicsId == topicId)
            .Include(r => r.Location)
            .Include(r => r.Reporter)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
            .Include(r => r.ImpactUrls)
            .OrderByDescending(r => r.ReportedAt)
            .ToListAsync();
    }
    public async Task AddPostAsync(DisastersReport report)
    {
        await _context.DisastersReports.AddAsync(report);
    }

    public async Task UpdatePostAsync(DisastersReport report)
    {
        _context.DisastersReports.Update(report);
        await Task.CompletedTask;
    }

    public async Task HardDeleteReportAsync(DisastersReport report)
    {
        _context.DisastersReports.Remove(report);
        await Task.CompletedTask;
    }
    public async Task<List<DisasterReportMapDto>> GetFilteredDisasterReportsAsync(ReportFilterDto filter)
    {
        return await _context.Set<DisasterReportMapDto>()
            .FromSqlInterpolated($@"
                EXEC sp_GetFilteredDisasterReportsForMap 
                    @TopicId = {filter.TopicId}, 
                    @TownshipName = {filter.TownshipName}, 
                    @RegionName = {filter.RegionName}, 
                    @StartDate = {filter.StartDate}, 
                    @EndDate = {filter.EndDate}, 
                    @IsUrgent = {filter.IsUrgent}")
            .ToListAsync();
    }
    public async Task<List<CategoryCountDto>> GetCategoryCountsAsync(int? year = null, int? month = null)
    {
        var yearParam = year.HasValue ? year.Value.ToString() : "NULL";
        var monthParam = month.HasValue ? month.Value.ToString() : "NULL";

        string sql = $"EXEC GetCategoryCounts @Year={yearParam}, @Month={monthParam}";

        return await _context.CategoryCountDtos.FromSqlRaw(sql).ToListAsync();
    }
    public async Task<List<(DateTime ReportDate, int ReportCount)>> GetReportCountLast7DaysAsync()
    {
        DateTime today = DateTime.UtcNow.Date;
        DateTime last7Days = today.AddDays(-6);

        var result = await _context.DisastersReports
            .Where(r => r.ReportedAt.Date >= last7Days && !r.IsDeleted)
            .GroupBy(r => r.ReportedAt.Date)
            .Select(g => new
            {
                ReportDate = g.Key,
                ReportCount = g.Count()
            })
            .OrderBy(x => x.ReportDate)
            .ToListAsync();

        // Convert anonymous type to tuple for returning from repository
        return result.Select(x => (x.ReportDate, x.ReportCount)).ToList();
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public ApplicationDBContext DbContext => _context;
}