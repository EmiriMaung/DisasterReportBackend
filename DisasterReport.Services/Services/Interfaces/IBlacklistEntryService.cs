using DisasterReport.Data.Domain;
using DisasterReport.Services.Models.BlacklistEntryDTO;
using DisasterReport.Services.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IBlacklistEntryService
    {
        Task<PaginatedResult<BlacklistEntryDto>> GetAllBlacklistEntriesAsync(
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


        Task<BlacklistStatsDto> GetBlacklistStatsAsync();

        Task<List<BlacklistExportDto>> GetAllBlacklistForExportAsync();

        Task<BlacklistEntryDto> GetBlacklistEntryByIdAsync(int id);

        //Task<IEnumerable<BlacklistHistoryDto>> GetUserBlacklistHistoryAsync(Guid userId);

        Task<BlacklistDetailDto> GetBlacklistDetailByIdAsync(int id);

        Task AddAsync(CreateBlacklistEntryDto dto);

        Task UpdateReasonAsync(int id, UnblockUserDto dto);

        Task SoftDeleteAsync(Guid id, Guid adminId, string unblockedReason);

        Task<bool> IsUserBlacklistedAsync(Guid userId);
    }
}
