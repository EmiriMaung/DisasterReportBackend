using DisasterReport.Data.Domain;

namespace DisasterReport.Data.Repositories.Interfaces
{
    public interface IReportRepo
    {
        Task<(List<Report> Items, int TotalCount)> GetAllAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder,
            string? statusFilter,
            DateTime? startDate,
            DateTime? endDate,
            Guid? adminId
        );

        Task<Report?> GetByIdAsync(int id);

        Task<Dictionary<string, int>> GetStatsAsync();

        Task<Report> CreateAsync(Report report);

        Task<Report?> ResolveAsync(int id, Guid adminId, string actionTaken);

        Task<Report?> RejectAsync(int id, Guid adminId);

        Task<bool> DeleteAsync(int id);
    }
}
