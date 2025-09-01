using DisasterReport.Data.Domain;

namespace DisasterReport.Data.Repositories
{
    public interface IDonationRepo
    {
        // ✅ Create
        Task<Donation> AddAsync(Donation donation);

        // ✅ Read
        Task<Donation?> GetByIdAsync(int id);
        Task<Donation?> GetByDonateRequestIdAsync(int donateRequestId);
        Task<IEnumerable<Donation>> GetAllAsync();
        Task<IEnumerable<Donation>> GetByOrganizationIdAsync(int organizationId);
        Task<IEnumerable<Donation>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Donation>> GetPlatformDonationsAsync();

        // ✅ Update
        Task UpdateAsync(Donation donation);

        // ✅ Delete (admin only)
        Task DeleteAsync(int id);
        // ✅ New: just return donations, Service will do grouping & mapping
        Task<IEnumerable<Donation>> GetAllWithOrganizationsAsync();
    }
}
