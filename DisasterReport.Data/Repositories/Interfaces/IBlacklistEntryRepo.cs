using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories.Interfaces
{
    public interface IBlacklistEntryRepo
    {
        Task<(List<BlacklistEntry> Items, int TotalCount)> GetAllAsync(
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

        Task<(int TotalBlocked, int TotalUnlocked, int BlockedLast7Days, int UnblockedLast7Days)> GetBlacklistStatsAsync();

        Task<List<BlacklistEntry>> GetAllForExportAsync();

        Task<BlacklistEntry?> GetByIdAsync(int id);

        Task<Dictionary<Guid, string>> GetUserNamesByIdsAsync(IEnumerable<Guid> userIds);

        Task<IEnumerable<BlacklistEntry>> GetBlacklistEntriesByUserIdAsync(Guid userId);

        Task AddAsync(BlacklistEntry entry);

        Task UpdateAsync(BlacklistEntry entry);

        Task SoftDeleteByUserIdAsync(Guid id, Guid adminId, string unblockedReason);

        Task<bool> IsUserBlacklistedAsync(Guid userId);
    }
}
