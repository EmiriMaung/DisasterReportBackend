using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories
{
    public interface IOrganizationDocRepo
    {
        Task AddAsync(OrganizationDoc doc);
        Task<IEnumerable<OrganizationDoc>> GetByOrganizationIdAsync(int orgId);
        Task<bool> SaveChangesAsync();
    }
}
