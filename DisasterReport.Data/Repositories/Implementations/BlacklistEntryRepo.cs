using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DisasterReport.Data.Repositories.Implementations
{
    public class BlacklistEntryRepo : IBlacklistEntryRepo
    {
        private readonly ApplicationDBContext _context;
        public BlacklistEntryRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlacklistEntry>> GetAllAsync()
        {
            return await _context.BlacklistEntries
                                .Include(be => be.User)
                                .AsNoTracking()
                                .ToListAsync();
        }


        public async Task<BlacklistEntry?> GetByIdAsync(int id)
        {
            return await _context.BlacklistEntries
                .Include(be => be.User)
                .FirstOrDefaultAsync(be => be.Id == id);
        }


        public async Task AddAsync(BlacklistEntry entry)
        {
            await _context.BlacklistEntries.AddAsync(entry);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(BlacklistEntry entry)
        {
            _context.BlacklistEntries.Update(entry);
            await _context.SaveChangesAsync();
        }


        public async Task SoftDeleteAsync(int id, Guid adminId)
        {
            var entry = await _context.BlacklistEntries.FindAsync(id);

            if (entry != null && !entry.IsDeleted)
            {
                entry.IsDeleted = true;
                entry.UpdatedAdminId = adminId;
                entry.UpdateAt = DateTime.UtcNow;

                _context.BlacklistEntries.Update(entry);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> IsUserBlacklistedAsync(Guid userId)
        {
            return await _context.BlacklistEntries
                                .AnyAsync(be => be.UserId == userId);
        }
    }
}