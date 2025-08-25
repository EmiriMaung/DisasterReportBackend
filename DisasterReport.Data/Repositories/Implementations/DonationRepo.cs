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

        //Create
        public async Task<Donation> AddAsync(Donation donation)
        {
            await _db.Donations.AddAsync(donation);
            await _db.SaveChangesAsync();
            return donation;
        }

        //Read
        public Task<IEnumerable<Donation>> GetAllAsync()
        {
            return Task.FromResult(_db.Donations
                .Include(d => d.DonateRequest)
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
                                       .AsEnumerable());
        }

        public Task<IEnumerable<Donation>> GetPlatformDonationsAsync()
        {
            return Task.FromResult(_db.Donations
                                       .Where(d => d.DonateRequest != null && d.DonateRequest.IsPlatformDonation)
                                       .Include(d => d.DonateRequest)
                                       .AsEnumerable());
        }

        //Update
        public async Task UpdateAsync(Donation donation)
        {
            _db.Donations.Update(donation);
            await _db.SaveChangesAsync();
        }

        //Delete
        public async Task DeleteAsync(int id)
        {
            var donation = await _db.Donations.FindAsync(id);
            if (donation != null)
            {
                _db.Donations.Remove(donation);
                await _db.SaveChangesAsync();
            }
        }
    }
}
