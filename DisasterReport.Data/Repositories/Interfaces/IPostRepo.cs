using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories.Interfaces
{
    public interface IPostRepo
    {
        Task<List<DisastersReport>> GetAllPostsWithMaterialsAsync();
        Task<List<DisastersReport>> GetAllPostsByReporterId(Guid reporterId);
        Task<List<DisastersReport>> GetPendingRejectReportByReporterIdAsync(Guid reporterId);
        Task<List<DisastersReport>> GetDeletedPostsByReporterId(Guid reporterId);
        Task<DisastersReport?> GetPostByIdAsync(int id);
        Task<(int total, int approve, int pending, int reject)> GetReportCountsByStatusAsync();
        Task<List<DisastersReport>> GetReportsByStatusAsync(int? status);
        Task<List<DisastersReport>> GetReportsByRegionAsync(string regionName);
        Task<List<DisastersReport>> GetReportsByTownshipAsync(string townshipName);
        Task<List<DisastersReport>> GetPendingReportsAsync();
        Task<List<DisastersReport>> GetUrgentReportsAsync();
        Task<List<DisastersReport>> GetReportsByOrganizationIdAsync(int organizationId);//for organization 
        Task AddPostAsync(DisastersReport report);
        Task UpdatePostAsync(DisastersReport report);
        Task SoftDeleteReportAsync(int reportId);
        Task RestoreDeletedReportAsync(int reportId);
        Task HardDeleteReportAsync(DisastersReport report);
        Task<List<DisastersReport>> SearchReportsAsync(string? keyword, string? category, string? region,string? township, bool? isUrgent, int? topicId);
        Task ApproveReportAsync(int reportId, Guid approvedBy);
        Task RejectReportAsync(int reportId, Guid rejectedBy);
        Task<List<DisastersReport>> GetReportsByTopicIdAsync(int topicId);
        Task<List<DisasterReportMapDto>> GetFilteredDisasterReportsAsync(ReportFilterDto filter);//for sp
        Task<List<CategoryCountDto>> GetCategoryCountsAsync(int? year = null, int? month = null);
        Task<List<(DateTime ReportDate, int ReportCount)>> GetReportCountLast7DaysAsync();
        Task SaveChangesAsync();
        ApplicationDBContext DbContext { get; }

    }

}
