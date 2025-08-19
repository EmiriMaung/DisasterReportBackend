using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
using DisasterReport.Services.Enums;
using DisasterReport.Services.Models;
using Microsoft.AspNetCore.Http;

namespace DisasterReport.Services.Services
{
    public class DonateRequestService : IDonateRequestService
    {
        private readonly IDonateRequestRepo _requestRepo;
        private readonly IDonationRepo _donationRepo;
        private readonly ICloudinaryService _cloudinaryService;

        public DonateRequestService(IDonateRequestRepo requestRepo,
                                    IDonationRepo donationRepo,
                                    ICloudinaryService cloudinaryService)
        {
            _requestRepo = requestRepo;
            _donationRepo = donationRepo;
            _cloudinaryService = cloudinaryService;
        }

        // ✅ Create request
        public async Task<DonateRequestReadDto> CreateAsync(DonateRequestCreateDto dto, IFormFile? slipFile, Guid userId)
        {
            string? slipUrl = null;
            string? fileType = null;
            int? fileSize = null;

            if (slipFile != null)
            {
                var uploadResult = await _cloudinaryService.UploadFileAsync(slipFile);
                slipUrl = uploadResult.SecureUrl;
                fileType = uploadResult.FileType;
                fileSize = (int)(slipFile.Length / 1024);
            }

            var request = new DonateRequest
            {
                RequestedByUserId = userId,
                Description = dto.Description,
                SupportType = dto.SupportType,
                Amount = dto.Amount,
                PaymentSlipUrl = slipUrl,
                FileType = fileType,
                FileSizeKb = fileSize,
                OrganizationId = dto.OrganizationId,
                IsPlatformDonation = dto.IsPlatformDonation,
                Status = (int)Status.Pending
            };

            // Save request first
            await _requestRepo.AddAsync(request);

            // Then handle donation if platform donation
            if (dto.IsPlatformDonation)
            {
                request.Status = (int)Status.Approved;
                request.DonatedAt = DateTime.UtcNow;

                var donation = new Donation
                {
                    DonateRequestId = request.Id,
                    DonatedAt = request.DonatedAt
                };
                await _donationRepo.AddAsync(donation);

                // Update request after donation
                await _requestRepo.UpdateAsync(request);
            }

            // Reload the request with navigation property populated
            var createdRequest = await _requestRepo.GetByIdAsync(request.Id);

            return MapToReadDto(createdRequest!);
        }

        // ✅ Review request (by organization)
        public async Task<DonateRequestReadDto?> ReviewAsync(int requestId, DonateRequestReviewDto dto)
        {
            var request = await _requestRepo.GetByIdAsync(requestId);
            if (request == null) return null;

            if (dto.Accept)
            {
                request.Status = (int)Status.Approved;
                request.DonatedAt = DateTime.UtcNow;

                var donation = new Donation
                {
                    DonateRequestId = request.Id,
                    DonatedAt = request.DonatedAt
                };
                await _donationRepo.AddAsync(donation);
            }
            else
            {
                request.Status = (int)Status.Rejected;
            }

            await _requestRepo.UpdateAsync(request);
            return MapToReadDto(request);
        }

        // ✅ Approve directly
        public async Task<DonateRequestReadDto?> ApproveAsync(int requestId)
        {
            var request = await _requestRepo.GetByIdAsync(requestId);
            if (request == null) return null;

            request.Status = (int)Status.Approved;
            request.DonatedAt = DateTime.UtcNow;

            var donation = new Donation
            {
                DonateRequestId = request.Id,
                DonatedAt = DateTime.UtcNow
            };
            await _donationRepo.AddAsync(donation);

            await _requestRepo.UpdateAsync(request);
            return MapToReadDto(request);
        }

        // ✅ Reject directly
        public async Task<DonateRequestReadDto?> RejectAsync(int requestId)
        {
            var request = await _requestRepo.GetByIdAsync(requestId);
            if (request == null) return null;

            request.Status = (int)Status.Rejected;
            await _requestRepo.UpdateAsync(request);

            return MapToReadDto(request);
        }

        // ✅ Get all
        public async Task<IEnumerable<DonateRequestReadDto>> GetAllAsync()
        {
            var requests = await _requestRepo.GetAllAsync();
            return requests.Select(MapToReadDto);
        }
        // ✅ Get by platform/organization donation
        public async Task<IEnumerable<DonateRequestReadDto>> GetByIsPlatformAsync(bool isPlatformDonation)
        {
            var requests = await _requestRepo.GetByIsPlatformAsync(isPlatformDonation);
            return requests.Select(MapToReadDto);
        }

        // ✅ Get by user
        public async Task<IEnumerable<DonateRequestReadDto>> GetByUserIdAsync(Guid userId)
        {
            var requests = await _requestRepo.GetByUserIdAsync(userId);
            return requests.Select(MapToReadDto);
        }

        // ✅ Get pending by org
        public async Task<IEnumerable<DonateRequestReadDto>> GetPendingByOrganizationIdAsync(int organizationId)
        {
            var requests = await _requestRepo.GetByOrganizationIdAsync(organizationId);
            return requests
                .Where(r => r.Status == (int)Status.Pending)
                .Select(MapToReadDto);
        }

        // ✅ Manual mapper
        private static DonateRequestReadDto MapToReadDto(DonateRequest request)
        {
            return new DonateRequestReadDto
            {
                Id = request.Id,
                RequestedByUserId = request.RequestedByUserId,
                RequestedByUserName = request.RequestedByUser?.Name,
                Description = request.Description,
                SupportType = request.SupportType,
                Amount = request.Amount,
                PaymentSlipUrl = request.PaymentSlipUrl,
                OrganizationId = request.OrganizationId,
                IsPlatformDonation = request.IsPlatformDonation,
                Status = (Status)request.Status,
                DonatedAt = request.Status == (int)Status.Approved ? request.DonatedAt : null
            };
        }
    }
}
