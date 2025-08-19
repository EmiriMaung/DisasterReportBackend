using DisasterReport.Services.Models;
using Microsoft.AspNetCore.Http;

namespace DisasterReport.Services.Services
{
    public interface IDonateRequestService
    {
        // Create a new donate request
        Task<DonateRequestReadDto> CreateAsync(DonateRequestCreateDto dto, IFormFile? slipFile, Guid userId);

        // Review (approve/reject) a donate request
        Task<DonateRequestReadDto?> ReviewAsync(int requestId, DonateRequestReviewDto dto);

        // Fetch requests
        Task<IEnumerable<DonateRequestReadDto>> GetAllAsync();
        Task<IEnumerable<DonateRequestReadDto>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<DonateRequestReadDto>> GetPendingByOrganizationIdAsync(int organizationId);

        // Approve request (auto creates donation)
        Task<DonateRequestReadDto?> ApproveAsync(int requestId);

        // Reject request
        Task<DonateRequestReadDto?> RejectAsync(int requestId);
    }
}
