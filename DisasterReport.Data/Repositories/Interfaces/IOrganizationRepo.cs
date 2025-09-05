using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories
{
    public interface IOrganizationRepo
    {
        Task<IEnumerable<Organization>> GetAllWithMembersAndDocsAsync(); 
        Task<IEnumerable<Organization>> GetAllWithMembersAndDocsBlackListedOrgsAsync();
        Task<IEnumerable<Organization>> GetAllAsync(); 
        Task<IEnumerable<Organization>> GetAllBlackListedOrgsAsync();
        Task<IEnumerable<Organization>> GetPendingOrgsAsync();
        Task<IEnumerable<Organization>> GetRejectedOrgsAsync();
        Task<IEnumerable<Organization>> GetByNameAsync(string orgName); 
        Task<IEnumerable<Organization>> GetByNameBlackListedOrgsAsync(string orgName);
        Task<Organization?> GetByIdAsync(int id);
        Task<Organization?> GetDetailsByIdAsync(int id); 
        Task<IEnumerable<Organization>> GetByUserIdAsync(Guid userId);
        Task<int> GetActiveOrganizationCountAsync();
        Task AddAsync(Organization organization);
        void Update(Organization organization);
        void Delete(Organization organization);
        Task<bool> SaveChangesAsync();
    }
}
