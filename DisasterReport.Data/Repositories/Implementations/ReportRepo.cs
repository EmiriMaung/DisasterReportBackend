using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories.Implementations
{
    public class ReportRepo : IReportRepo
    {
        private readonly ApplicationDBContext _context;
        public ReportRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<(List<Report> Items, int TotalCount)> GetAllAsync(
        int page,
        int pageSize,
        string? searchQuery,
        string? sortBy,
        string? sortOrder,
        string? statusFilter,
        DateTime? startDate,
        DateTime? endDate,
        Guid? adminId,
        string? reportTypeFilter
    )
        {
            var query = _context.Reports
                .Include(r => r.Reporter)
                .Include(r => r.ReportedUser)
                .Include(r => r.ReportedPost)
                .AsNoTracking();

            // Search
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var pattern = $"%{searchQuery.Trim()}%";
                query = query.Where(r =>
                    EF.Functions.Like(r.Reason, pattern) ||
                    EF.Functions.Like(r.Reporter.Name, pattern) ||
                    (r.ReportedUser != null && EF.Functions.Like(r.ReportedUser.Name, pattern))
                );
            }

            // Status filter
            if (!string.IsNullOrEmpty(statusFilter) && statusFilter != "All")
            {
                query = query.Where(r => r.Status == statusFilter);
            }

            if (!string.IsNullOrEmpty(reportTypeFilter))
            {
                if (reportTypeFilter.Equals("Post", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(r => r.ReportedPostId != null);
                }
                else if (reportTypeFilter.Equals("User", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(r => r.ReportedUserId != null);
                }
            }

            // Date range filter
            if (startDate.HasValue)
                query = query.Where(r => r.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(r => r.CreatedAt < endDate.Value.AddDays(1));

            // Admin filter (ReviewedBy)
            if (adminId.HasValue)
                query = query.Where(r => r.ReviewedBy == adminId.Value);

            // Sorting
            query = sortBy?.ToLowerInvariant() switch
            {
                "reason" => sortOrder?.ToLowerInvariant() == "desc"
                    ? query.OrderByDescending(r => r.Reason)
                    : query.OrderBy(r => r.Reason),

                "reporter" => sortOrder?.ToLowerInvariant() == "desc"
                    ? query.OrderByDescending(r => r.Reporter.Name)
                    : query.OrderBy(r => r.Reporter.Name),

                "date" => sortOrder?.ToLowerInvariant() == "asc"
                    ? query.OrderBy(r => r.CreatedAt)
                    : query.OrderByDescending(r => r.CreatedAt),

                _ => query.OrderByDescending(r => r.CreatedAt)
            };

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }


        public async Task<Report?> GetByIdAsync(int id)
        {
            return await _context.Reports
                .Include(r => r.Reporter)
                .Include(r => r.ReportedUser)
                .Include(r => r.ReportedPost)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<Dictionary<string, int>> GetStatsAsync()
        {
            var stats = new Dictionary<string, int>
            {
                ["Total"] = await _context.Reports.CountAsync(),
                ["PendingReports"] = await _context.Reports.CountAsync(r => r.Status == "Pending"),
                ["Resolved"] = await _context.Reports.CountAsync(r => r.Status == "Resolved"),
                ["Reject"] = await _context.Reports.CountAsync(r => r.Status == "Rejected")
            };

            return stats;
        }

        public async Task<Report> CreateAsync(Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();
            return report;
        }


        public async Task<Report?> ResolveAsync(int id, Guid adminId, string actionTaken)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null || report.Status != "Pending") return null;

            report.Status = "Resolved";
            report.ActionTaken = actionTaken;
            report.ReviewedBy = adminId;
            report.ReviewedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return report;
        }


        public async Task<Report?> RejectAsync(int id, Guid adminId)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null || report.Status != "Pending") return null;

            report.Status = "Rejected";
            report.ReviewedBy = adminId;
            report.ReviewedAt = DateTime.UtcNow;
            report.ActionTaken = "No Action taken.";

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("RejectAsync failed: " + ex.Message, ex);
            }

            return report;
        }
        // In ReportRepo.cs

        //public async Task<Report?> RejectAsync(int id, Guid adminId)
        //{
        //    // First, check if the report exists and is pending
        //    var reportExists = await _context.Reports
        //        .AsNoTracking() // Use AsNoTracking for a fast, read-only check
        //        .AnyAsync(r => r.Id == id && r.Status == "Pending");

        //    if (!reportExists)
        //    {
        //        return null;
        //    }

        //    // Create a "stub" entity with only the ID.
        //    // This represents the record we want to update.
        //    var reportToUpdate = new Report { Id = id };

        //    // Attach the stub to the context. EF now knows about this record
        //    // but isn't tracking its original values.
        //    _context.Reports.Attach(reportToUpdate);

        //    // Now, explicitly tell EF which properties have changed.
        //    reportToUpdate.Status = "Rejected";
        //    reportToUpdate.ReviewedBy = adminId;
        //    reportToUpdate.ReviewedAt = DateTime.UtcNow;

        //    // EF will now generate a clean UPDATE statement for only these three fields.
        //    await _context.SaveChangesAsync();

        //    // Return the updated entity
        //    return reportToUpdate;
        //}

        public async Task<bool> DeleteAsync(int id)
        {
            var report = await _context.Reports.FindAsync(id);
            if (report == null) return false;

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
