using DisasterReport.Data.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories
{
    public class OrganizationRepo : IOrganizationRepo
    {
        private readonly ApplicationDBContext _context;
        public OrganizationRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Organization>> GetAllWithMembersAndDocsAsync()
        {
            return await _context.Organizations
                .Where(o => !o.IsBlackListedOrg)
                .Include(o => o.OrganizationMembers)
                .Include(o => o.OrganizationDocs)
                .ToListAsync();
        }

        public async Task<IEnumerable<Organization>> GetAllWithMembersAndDocsBlackListedOrgsAsync()
        {
            return await _context.Organizations
               .Where(o => o.IsBlackListedOrg)
               .Include(o => o.OrganizationMembers)
               .Include(o => o.OrganizationDocs)
               .ToListAsync();
        }

        public async Task<IEnumerable<Organization>> GetAllAsync()
        {
            return await _context.Organizations
                .ToListAsync();
        }

        public async Task<IEnumerable<Organization>> GetAllBlackListedOrgsAsync()
        {
            return await _context.Organizations
                .Where(o => o.IsBlackListedOrg)
                .ToListAsync();
        }

        public async Task<IEnumerable<Organization>> GetPendingOrgsAsync()
        {
            return await _context.Organizations
                .Where(o => o.Status == 0)
                .ToListAsync();
        }
        public async Task<IEnumerable<Organization>> GetRejectedOrgsAsync()
        {
            return await _context.Organizations
                .Where(o => o.Status == 2)
                .ToListAsync();
        }
        public async Task<IEnumerable<Organization>> GetByNameAsync(string orgName)
        {
            return await _context.Organizations
                 .Where(o => !o.IsBlackListedOrg && o.Name.Contains(orgName))
                 .ToListAsync();
        }

        public async Task<IEnumerable<Organization>> GetByNameBlackListedOrgsAsync(string orgName)
        {
            return await _context.Organizations
                .Where(o => o.IsBlackListedOrg && o.Name.Contains(orgName))
                .ToListAsync();
        }

        public async Task<IEnumerable<Organization>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Organizations
                .Where(o => o.OrganizationMembers.Any(om => om.UserId == userId))
                .ToListAsync();
        }

        public async Task<Organization?> GetByIdAsync(int id)
        {
            return await _context.Organizations
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<Organization?> GetDetailsByIdAsync(int id)
        {
            return await _context.Organizations
                .Include(o => o.OrganizationMembers)
                .Include(o => o.OrganizationDocs)
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<int> GetActiveOrganizationCountAsync()
        {
            return await _context.Organizations
                .Where(o => !o.IsBlackListedOrg && o.Status == 1)
                .CountAsync();
        }

        public async Task AddAsync(Organization organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization), "Organization cannot be null");
            }
            await _context.Organizations.AddAsync(organization);
        }

        public void Update(Organization organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization), "Organization cannot be null");
            }
            _context.Organizations.Update(organization);
        }

        public void Delete(Organization organization)
        {
            if (organization == null)
            {
                throw new ArgumentNullException(nameof(organization), "Organization cannot be null");
            }
            _context.Organizations.Remove(organization);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

    }
}
