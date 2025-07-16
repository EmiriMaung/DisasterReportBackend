using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            .Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
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
            .Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
            .ToListAsync();
    }

    public async Task<List<DisastersReport>> GetAllPostsWithMaterialsAsync()
    {
        return await _context.DisastersReports
            .Where(r => r.Status == 1)
            .AsNoTracking()
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            .Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes)
            .ToListAsync();
    }

    public async Task<List<DisastersReport>> GetPendingReportsAsync()
    {
        return await _context.DisastersReports.Where(r => r.Status == 0).Include(r => r.Location)
            .AsNoTracking()
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            .Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes).ToListAsync();
    }

    public async Task<List<DisastersReport>> GetUrgentReportsAsync()
    {
        return await _context.DisastersReports.Where(r => r.IsUrgent && !r.IsDeleted && r.Status == 1).Include(r => r.Location)
            .AsNoTracking()
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            .Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes).ToListAsync();
    }

    public async Task<List<DisastersReport>> GetReportsByStatusAsync(int status)
    {
        return await _context.DisastersReports.Where(r => r.Status == status && !r.IsDeleted)
            .AsNoTracking()
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            .Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes).ToListAsync();
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
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<DisastersReport>> SearchReportsAsync(string? keyword, string? category, string? region, bool? isUrgent)
    {
        var query = _context.DisastersReports.AsQueryable();

        if (!string.IsNullOrEmpty(keyword))
            query = query.Where(r => r.Title.Contains(keyword) || r.Description.Contains(keyword));

        if (!string.IsNullOrEmpty(category))
            query = query.Where(r => r.Category == category);

        if (!string.IsNullOrEmpty(region))
            query = query.Where(r => r.Location.RegionName == region);

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
            report.UpdatedAt = DateTime.UtcNow;
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
            .Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes).ToListAsync();
    }

    public async Task<List<DisastersReport>> GetReportsByTownshipAsync(string townshipName)
    {
        return await _context.DisastersReports.Where(r => r.Location.TownshipName == townshipName && !r.IsDeleted && r.Status == 1)
            .Include(r => r.Location)
            .Include(r => r.Comments)
            .Include(r => r.ImpactUrls)
            .Include(r => r.DonateRequests)
            .Include(r => r.DisasterTopics)
            .Include(r => r.ImpactTypes)
            .Include(r => r.SupportTypes).ToListAsync();
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

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
    public ApplicationDBContext DbContext => _context;
}