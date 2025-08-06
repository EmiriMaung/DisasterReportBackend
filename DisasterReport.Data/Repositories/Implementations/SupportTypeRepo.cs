using DisasterReport.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace DisasterReport.Data.Repositories
{
    public class SupportTypeRepo : ISupportTypeRepo
    {
        private readonly ApplicationDBContext _context;

        public SupportTypeRepo(ApplicationDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<SupportType>> GetAllAsync()
        {
            return await _context.SupportTypes
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<SupportType?> GetByIdAsync(int id)
        {
            return await _context.SupportTypes.FindAsync(id);
        }

        public async Task<SupportType> AddAsync(SupportType supportType)
        {
            _context.SupportTypes.Add(supportType);
            await _context.SaveChangesAsync();
            return supportType;
        }

        public async Task<SupportType?> UpdateAsync(SupportType supportType)
        {
            var existing = await _context.SupportTypes.FindAsync(supportType.Id);
            if (existing == null) return null;

            existing.Name = supportType.Name;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var supportType = await _context.SupportTypes.FindAsync(id);
            if (supportType == null) return false;

            _context.SupportTypes.Remove(supportType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
