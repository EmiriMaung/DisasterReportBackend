using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories.Implementations
{
    public class ImpactUrlRepo : IImpactUrlRepo
    {
        private readonly ApplicationDBContext _context;

        public ImpactUrlRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ImpactUrl url)
        {
            await _context.ImpactUrls.AddAsync(url);
        }

        public async Task DeleteAsync(int id)
        {
            var url = await _context.ImpactUrls.FindAsync(id);
            if (url != null)
            {
                _context.ImpactUrls.Remove(url);
            }
        }

        public async Task<ImpactUrl?> GetByIdAsync(int id)
        {
            return await _context.ImpactUrls.FindAsync(id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ImpactUrl url)
        {
            _context.ImpactUrls.Update(url);
        }
    }
}
