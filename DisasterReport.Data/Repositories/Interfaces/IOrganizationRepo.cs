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
        //Detail
        Task<IEnumerable<Organization>> GetAllWithMembersAndDocsAsync(); //BlackListed organizations are not included in this method
        Task<IEnumerable<Organization>> GetAllWithMembersAndDocsBlackListedOrgsAsync();
        //Summary
        Task<IEnumerable<Organization>> GetAllAsync(); //BlackListed organizations are not included in this method
        Task<IEnumerable<Organization>> GetAllBlackListedOrgsAsync();
        Task<IEnumerable<Organization>> GetPendingOrgsAsync();
        Task<IEnumerable<Organization>> GetRejectedOrgsAsync();
        Task<IEnumerable<Organization>> GetByNameAsync(string orgName); //BlackListed organizations are not included in this method
        Task<IEnumerable<Organization>> GetByNameBlackListedOrgsAsync(string orgName);
        Task<Organization?> GetByIdAsync(int id);
        Task<Organization?> GetDetailsByIdAsync(int id); // with members and docs
        Task<IEnumerable<Organization>> GetByUserIdAsync(Guid userId);
        Task AddAsync(Organization organization);
        void Update(Organization organization);
        void Delete(Organization organization);
        Task<bool> SaveChangesAsync();
    }
}
