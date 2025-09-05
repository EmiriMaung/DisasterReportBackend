using DisasterReport.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace DisasterReport.Data.Repositories
{
    public class DonateRequestRepo : IDonateRequestRepo
    {
        private readonly ApplicationDBContext _db;

        public DonateRequestRepo(ApplicationDBContext db)
        {
            _db = db;
        }

        public async Task<DonateRequest> AddAsync(DonateRequest request)
        {
            await _db.DonateRequests.AddAsync(request);
            await _db.SaveChangesAsync();
            return request;
        }

        public Task<IEnumerable<DonateRequest>> GetAllAsync()
        {
            return Task.FromResult(_db.DonateRequests
                                      .Include(r => r.Donations)
                                      .Include(r => r.RequestedByUser)
                                      .AsEnumerable());
        }

        public Task<DonateRequest?> GetByIdAsync(int id)
        {
            return _db.DonateRequests
                      .Include(r => r.Donations)
                      .Include(r => r.RequestedByUser)
                      .FirstOrDefaultAsync(r => r.Id == id);
        }

        public Task<IEnumerable<DonateRequest>> GetByOrganizationIdAsync(int organizationId)
        {
            return Task.FromResult(_db.DonateRequests
                                       .Where(r => r.OrganizationId == organizationId)
                                       .Include(r => r.Donations)
                                       .Include(r => r.RequestedByUser)
                                       .AsEnumerable());
        }

        public Task<IEnumerable<DonateRequest>> GetByUserIdAsync(Guid userId)
        {
            return Task.FromResult(_db.DonateRequests
                                        .Where(r => r.RequestedByUserId == userId)
                                        .Include(r => r.Donations)
                                        .AsEnumerable());
        }
        public Task<IEnumerable<DonateRequest>> GetByIsPlatformAsync(bool isPlatform)
        {
            return Task.FromResult(
                _db.DonateRequests
                    .Where(r => r.IsPlatformDonation == isPlatform)
                    .Include(r => r.Donations)
                    .Include(r => r.RequestedByUser)
                    .AsEnumerable()
            );
        }

        public async Task UpdateAsync(DonateRequest request)
        {
            _db.DonateRequests.Update(request);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, int status)
        {
            var request = await _db.DonateRequests.FindAsync(id);
            if (request != null)
            {
                request.Status = status;
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var request = await _db.DonateRequests.FindAsync(id);
            if (request != null)
            {
                _db.DonateRequests.Remove(request);
                await _db.SaveChangesAsync();
            }
        }
    }
}
