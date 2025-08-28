using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace DisasterReport.Data.Repositories.Implementations
{
    // File: DisasterReport.Data.Repositories.Interfaces/IDisasterEventRepo.cs
    public interface IDisasterEventRepo
    {
        Task<List<string>> GetExistingEventIdsAsync(List<string> eventIds);
        Task<List<DisasterEventNasa>> GetAllAsync();
        Task AddAsync(DisasterEventNasa entity);
        Task AddRangeAsync(List<DisasterEventNasa> entities); // Add this method for bulk insert
        Task SaveChangesAsync();
    }
    // File: DisasterReport.Data.Repositories.Implementations/DisasterEventRepo.cs
    public class DisasterEventRepo : IDisasterEventRepo
    {
        private readonly ApplicationDBContext _context;

        public DisasterEventRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<List<string>> GetExistingEventIdsAsync(List<string> eventIds)
        {
            return await _context.DisasterEventNasas
                .Where(e => eventIds.Contains(e.EventId))
                .Select(e => e.EventId)
                .ToListAsync();
        }

        public async Task<List<DisasterEventNasa>> GetAllAsync()
        {
            return await _context.DisasterEventNasas
                .OrderByDescending(e => e.EventDate).AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(DisasterEventNasa entity)
        {
            await _context.DisasterEventNasas.AddAsync(entity);
        }

        public async Task AddRangeAsync(List<DisasterEventNasa> entities)
        {
            await _context.DisasterEventNasas.AddRangeAsync(entities);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
