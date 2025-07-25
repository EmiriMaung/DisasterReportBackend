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
        Task<IEnumerable<BlacklistEntry>> GetAllAsync();

        Task<BlacklistEntry?> GetByIdAsync(int id);

        Task AddAsync(BlacklistEntry entry);

        Task UpdateAsync(BlacklistEntry entry);

        Task SoftDeleteAsync(int id, Guid adminId);

        Task<bool> IsUserBlacklistedAsync(Guid userId);
    }
}
