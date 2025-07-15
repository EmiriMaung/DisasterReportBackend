using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories
{
    public interface IOrganizationMemberRepo
    {
        Task AddAsync(OrganizationMember member);
        Task RemoveAsync(OrganizationMember member);
        Task<OrganizationMember?> GetByTokenAsync(Guid token); // for inviation acceptance
        Task<OrganizationMember?> GetByOrgAndUserAsync(int orgId, Guid userId);
        Task<IEnumerable<OrganizationMember>> GetByOrganizationIdAsync(int orgId);
        Task<bool> SaveChangesAsync();
    }
}
