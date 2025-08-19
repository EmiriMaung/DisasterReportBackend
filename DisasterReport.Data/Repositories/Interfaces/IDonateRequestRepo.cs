using DisasterReport.Data.Domain;

namespace DisasterReport.Data.Repositories
{
    public interface IDonateRequestRepo
    {
        // ✅ Create
        Task<DonateRequest> AddAsync(DonateRequest request);

        // ✅ Read
        Task<DonateRequest?> GetByIdAsync(int id);
        Task<IEnumerable<DonateRequest>> GetAllAsync();
        Task<IEnumerable<DonateRequest>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<DonateRequest>> GetByOrganizationIdAsync(int organizationId);
        Task<IEnumerable<DonateRequest>> GetByIsPlatformAsync(bool isPlatform);

        // ✅ Update
        Task UpdateAsync(DonateRequest request);

        // ✅ Delete (not common in finance, but keep for admin)
        Task DeleteAsync(int id);

        // ✅ Change Status (approve / reject)
        Task UpdateStatusAsync(int id, int status);
    }
}
