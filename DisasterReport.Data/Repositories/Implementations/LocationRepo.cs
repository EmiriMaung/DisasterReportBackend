using DisasterReport.Data.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories.Implementations
{
    public class LocationRepo : ILocationRepo
    {
        private readonly ApplicationDBContext _context;
        public LocationRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<Location?> GetByIdAsync(int id)
        {
            return await _context.Locations.FindAsync(id);
        }

        public async Task AddAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Location location)
        {
             _context.Locations.Update(location);
        }

        public async Task DeleteAsync(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            if (location != null)
            {
                _context.Locations.Remove(location);
            }
        }
    }
}
