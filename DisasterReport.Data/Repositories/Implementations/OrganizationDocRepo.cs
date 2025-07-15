using DisasterReport.Data.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories
{
    public class OrganizationDocRepo : IOrganizationDocRepo
    {
        private readonly ApplicationDBContext _context;
        public OrganizationDocRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task AddAsync(OrganizationDoc doc)
        {
            await _context.OrganizationDocs.AddAsync(doc);
        }

        public async Task<IEnumerable<OrganizationDoc>> GetByOrganizationIdAsync(int orgId)
        {
            return await _context.OrganizationDocs
            .Where(d => d.OrganizationId == orgId)
            .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
