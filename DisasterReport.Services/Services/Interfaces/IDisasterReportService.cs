using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisasterReport.Services.Models;

namespace DisasterReport.Services.Services.Interfaces;

public interface IDisasterReportService
{
    Task<IEnumerable<DisasterReportDto>> GetAllReportsAsync();

    Task<IEnumerable<DisasterReportDto>> GetUrgentReportsAsync();

    Task<IEnumerable<DisasterReportDto>> GetMyReportsAsync(Guid reporterId);
    Task<IEnumerable<DisasterReportDto>> GetMyDeletedReportsAsync(Guid reporterId);

    Task<IEnumerable<DisasterReportDto>> GetDeletedReportsAsync(string category);

    Task<DisasterReportDto?> GetReportByIdAsync(int id);

    Task AddReportAsync(AddDisasterReportDto report,Guid reporterId);

    Task UpdateReportAsync(int reportId, UpdateDisasterReportDto reportDto);

    Task SoftDeleteAsync(int id);

    Task RestoreReportAsync(int id);

    Task HardDeleteAsync(int id);

    Task<IEnumerable<DisasterReportDto>> GetReportsByStatusAsync(int status);

    Task<IEnumerable<DisasterReportDto>> SearchReportsAsync(string? keyword, string? category, string? region, bool? isUrgent);

    Task<IEnumerable<DisasterReportDto>> GetReportsByRegionAsync(string regionName);

    Task<IEnumerable<DisasterReportDto>> GetReportsByTownshipAsync(string townshipName);

    Task ApproveReportAsync(int reportId, Guid approvedBy);

    Task RejectReportAsync(int reportId, Guid rejectedBy);
}

