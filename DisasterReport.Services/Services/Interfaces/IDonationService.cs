using DisasterReport.Services.Models;

namespace DisasterReport.Services.Services
{
    public interface IDonationService
    {
        Task<IEnumerable<DonationReadDto>> GetAllAsync();
        Task<IEnumerable<DonationReadDto>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<DonationReadDto>> GetByOrganizationIdAsync(int organizationId);
        Task<decimal> GetTotalDonatedAmountAsync();

        // ✅ New Method: Group total donation by organization
        Task<IEnumerable<OrganizationDonationSummaryDto>> GetOrganizationDonationSummaryAsync();
    }
}
