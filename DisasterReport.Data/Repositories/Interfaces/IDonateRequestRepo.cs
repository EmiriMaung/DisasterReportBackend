using DisasterReport.Data.Domain;

namespace DisasterReport.Data.Repositories
{
    public interface IDonateRequestRepo
    {
        Task<DonateRequest> AddAsync(DonateRequest request);
        Task<DonateRequest?> GetByIdAsync(int id);
        Task<IEnumerable<DonateRequest>> GetAllAsync();
        Task<IEnumerable<DonateRequest>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<DonateRequest>> GetByOrganizationIdAsync(int organizationId);
        Task<IEnumerable<DonateRequest>> GetByIsPlatformAsync(bool isPlatform);
        Task UpdateAsync(DonateRequest request);
        Task DeleteAsync(int id);
        Task UpdateStatusAsync(int id, int status);
    }
}
