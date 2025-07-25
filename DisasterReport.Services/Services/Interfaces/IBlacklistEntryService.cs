using DisasterReport.Data.Domain;
using DisasterReport.Services.Models.BlacklistEntryDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IBlacklistEntryService
    {
        Task<IEnumerable<BlacklistEntryDto>> GetAllBlacklistEntriesAsync();

        Task<BlacklistEntryDto> GetBlacklistEntryByIdAsync(int id);

        Task AddAsync(CreateBlacklistEntryDto dto);

        Task UpdateReasonAsync(int id, UpdateBlacklistEntryDto dto);

        Task SoftDeleteAsync(int id, Guid adminId);

        Task<bool> IsUserBlacklistedAsync(Guid userId);
    }
}
