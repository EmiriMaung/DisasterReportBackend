using DisasterReport.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace DisasterReport.Data.Repositories
{
    public class DonationRepo : IDonationRepo
    {
        private readonly ApplicationDBContext _db;

        public DonationRepo(ApplicationDBContext db)
        {
            _db = db;
        }

        public async Task<Donation> AddAsync(Donation donation)
        {
            await _db.Donations.AddAsync(donation);
            await _db.SaveChangesAsync();
            return donation;
        }

        public Task<IEnumerable<Donation>> GetAllAsync()
        {
            return Task.FromResult(_db.Donations
                .Include(d => d.DonateRequest)
                .ThenInclude(d => d.RequestedByUser)
                .AsEnumerable());
        }

        public Task<Donation?> GetByDonateRequestIdAsync(int donateRequestId)
        {
            return _db.Donations
                      .FirstOrDefaultAsync(d => d.DonateRequestId == donateRequestId);
        }

        public Task<Donation?> GetByIdAsync(int id)
        {
            return _db.Donations
                      .Include(d => d.DonateRequest)
                      .FirstOrDefaultAsync(d => d.Id == id);
        }

        public Task<IEnumerable<Donation>> GetByOrganizationIdAsync(int organizationId)
        {
            return Task.FromResult(_db.Donations
                                        .Where(d => d.DonateRequest != null && d.DonateRequest.OrganizationId == organizationId)
                                        .Include(d => d.DonateRequest)
                                        .ThenInclude(d => d.RequestedByUser)
                                        .AsEnumerable());
        }

        public Task<IEnumerable<Donation>> GetByUserIdAsync(Guid userId)
        {
            return Task.FromResult(_db.Donations
                                       .Where(d => d.DonateRequest != null && d.DonateRequest.RequestedByUserId == userId)
                                       .Include(d => d.DonateRequest)
                                            .ThenInclude(d => d.RequestedByUser)
                                       .Include(d => d.DonateRequest)        // <-- include Organization too
                                            .ThenInclude(r => r.Organization)
                                       .AsEnumerable());
        }

        public Task<IEnumerable<Donation>> GetPlatformDonationsAsync()
        {
            return Task.FromResult(_db.Donations
                                       .Where(d => d.DonateRequest != null && d.DonateRequest.IsPlatformDonation)
                                       .Include(d => d.DonateRequest)
                                       .AsEnumerable());
        }

        public async Task UpdateAsync(Donation donation)
        {
            _db.Donations.Update(donation);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var donation = await _db.Donations.FindAsync(id);
            if (donation != null)
            {
                _db.Donations.Remove(donation);
                await _db.SaveChangesAsync();
            }
        }
        public Task<IEnumerable<Donation>> GetAllWithOrganizationsAsync()
        {
            return Task.FromResult(
                _db.Donations
                    .Include(d => d.DonateRequest)
                        .ThenInclude(r => r.Organization)
                    .AsEnumerable()
            );
        }
    }
}
