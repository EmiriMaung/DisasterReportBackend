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
    public class DisasterEventNasaRepo : IDisasterEventNasaRepo
    {
        private readonly ApplicationDBContext _context;

        public DisasterEventNasaRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string eventId)
        {
            return await _context.DisasterEventNasas
                .AnyAsync(e => e.EventId == eventId);
        }

        public async Task AddAsync(DisasterEventNasa entity)
        {
            await _context.DisasterEventNasas.AddAsync(entity);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
