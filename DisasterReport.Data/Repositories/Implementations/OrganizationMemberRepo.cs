using DisasterReport.Data.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories
{
    public class OrganizationMemberRepo : IOrganizationMemberRepo
    {
        private readonly ApplicationDBContext _context;
        public OrganizationMemberRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(OrganizationMember member)
        {
            await _context.OrganizationMembers.AddAsync(member);
        }

        public async Task RemoveAsync(OrganizationMember member)
        {
            _context.OrganizationMembers.Remove(member);
        }

        public async Task<OrganizationMember?> GetByOrgAndUserAsync(int orgId, Guid userId)
        {
            return await _context.OrganizationMembers
                         .FirstOrDefaultAsync(m => m.OrganizationId == orgId && m.UserId == userId);
        }

        public async Task<IEnumerable<OrganizationMember>> GetByOrganizationIdAsync(int orgId)
        {
            return await _context.OrganizationMembers
            .Where(m => m.OrganizationId == orgId)
            .ToListAsync();
        }

        public async Task<OrganizationMember?> GetByTokenAsync(Guid token)
        {
            return await _context.OrganizationMembers
                         .FirstOrDefaultAsync(m => m.InvitationToken == token);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
