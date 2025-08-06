using DisasterReport.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace DisasterReport.Data.Repositories
{
    public class ImpactTypeRepo : IImpactTypeRepo
    {
        private readonly ApplicationDBContext _context;

        public ImpactTypeRepo(ApplicationDBContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<ImpactType>> GetAllAsync()
        {
            return await _context.ImpactTypes
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<ImpactType?> GetByIdAsync(int id)
        {
            return await _context.ImpactTypes.FindAsync(id);
        }

        public async Task<ImpactType> AddAsync(ImpactType impactType)
        {
            _context.ImpactTypes.Add(impactType);
            await _context.SaveChangesAsync();
            return impactType;
        }

        public async Task<ImpactType?> UpdateAsync(ImpactType impactType)
        {
            var existing = await _context.ImpactTypes.FindAsync(impactType.Id);
            if (existing == null) return null;

            existing.Name = impactType.Name;
            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var impactType = await _context.ImpactTypes.FindAsync(id);
            if (impactType == null) return false;

            _context.ImpactTypes.Remove(impactType);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
