using DisasterReport.Services.Models;
using Microsoft.AspNetCore.Http;

namespace DisasterReport.Services.Services
{
    public interface IDonateRequestService
    {
        Task<DonateRequestReadDto> CreateAsync(DonateRequestCreateDto dto, IFormFile? slipFile, Guid userId);
        Task<DonateRequestReadDto?> ReviewAsync(int requestId, DonateRequestReviewDto dto);
        Task<IEnumerable<DonateRequestReadDto>> GetAllAsync();
        Task<IEnumerable<DonateRequestReadDto>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<DonateRequestReadDto>> GetPendingByOrganizationIdAsync(int organizationId);
        Task<IEnumerable<DonateRequestReadDto>> GetByIsPlatformAsync(bool isPlatformDonation);
        Task<DonateRequestReadDto?> ApproveAsync(int requestId);
        Task<DonateRequestReadDto?> RejectAsync(int requestId);
    }
}
