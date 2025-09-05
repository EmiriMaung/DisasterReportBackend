using DisasterReport.Data.Domain;

namespace DisasterReport.Data.Repositories
{
    public interface IDonationRepo
    {
        Task<Donation> AddAsync(Donation donation);
        Task<Donation?> GetByIdAsync(int id);
        Task<Donation?> GetByDonateRequestIdAsync(int donateRequestId);
        Task<IEnumerable<Donation>> GetAllAsync();
        Task<IEnumerable<Donation>> GetByOrganizationIdAsync(int organizationId);
        Task<IEnumerable<Donation>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Donation>> GetPlatformDonationsAsync();
        Task UpdateAsync(Donation donation);
        Task DeleteAsync(int id);
        Task<IEnumerable<Donation>> GetAllWithOrganizationsAsync();
    }
}
