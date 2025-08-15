using DisasterReport.Data.Domain;
using DisasterReport.Services.Models;
using DisasterReport.Services.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces;

public interface IDisasterReportService
{
    Task<IEnumerable<DisasterReportDto>> GetAllReportsAsync();

    Task<IEnumerable<DisasterReportDto>> GetUrgentReportsAsync();

    Task<IEnumerable<DisasterReportDto>> GetMyReportsAsync(Guid reporterId);

    Task<IEnumerable<DisasterReportDto>> GetMyDeletedReportsAsync(Guid reporterId);

    Task<IEnumerable<DisasterReportDto>> GetAllReportsByReporterIdAsync(Guid reporterId);
    Task<IEnumerable<DisasterReportDto>> GetPendingRejectReportByReporterIdAsync(Guid reporterId);

    Task<IEnumerable<DisasterReportDto>> GetDeletedReportsByReporterIdAsync(Guid reporterId);

    Task<IEnumerable<DisasterReportDto>> GetDeletedReportsAsync(string category);

    Task<DisasterReportDto?> GetReportByIdAsync(int id);

    Task AddReportAsync(AddDisasterReportDto report,Guid reporterId);

    Task UpdateReportAsync(int reportId, UpdateDisasterReportDto reportDto);

    Task SoftDeleteAsync(int id);

    Task RestoreReportAsync(int id);

    Task HardDeleteAsync(int id);

    Task<ReportStatusCountDto> CountReportsByStatusAsync();

    Task<PagedResponse<DisasterReportDto>> GetReportsByStatusAsync(int? status, int pageNumber = 1, int pageSize = 18);

    Task<IEnumerable<DisasterReportDto>> SearchReportsAsync(string? keyword, string? category, string? region, string? township, bool? isUrgent, int? topicId);

    Task<IEnumerable<DisasterReportDto>> GetReportsByRegionAsync(string regionName);

    Task<IEnumerable<DisasterReportDto>> GetReportsByTownshipAsync(string townshipName);

    Task<IEnumerable<DisasterReportDto>> GetReportsByTopicIdAsync(int topicId);

    //Task ApproveReportAsync(int reportId, ApproveWithTopicDto topicDto);
    Task ApproveReportAsync(int reportId, ApproveWithTopicDto topicDto, Guid adminId);

    Task RejectReportAsync(int reportId, Guid rejectedBy);

    Task<IEnumerable<DisasterReportDto>> GetRelatedReportsByTopicAsync(int reportId);

    Task<List<CategoryCountDto>> GetCategoryCountsAsync(int? year = null, int? month = null);

    Task<List<(DateTime ReportDate, int ReportCount)>> GetReportCountLast7DaysAsync();

    Task<List<DisasterReportMapDto>> GetDisasterReportsForMapAsync(ReportFilterDto filter);

}

