using DisasterReport.Data.Repositories;
using DisasterReport.Services.Models;

namespace DisasterReport.Services.Services
{
    public class DonationService : IDonationService
    {
        private readonly IDonationRepo _donationRepo;

        public DonationService(IDonationRepo donationRepo)
        {
            _donationRepo = donationRepo;
        }

        public async Task<IEnumerable<DonationReadDto>> GetAllAsync()
        {
            var donations = await _donationRepo.GetAllAsync();
            return donations.Select(d => new DonationReadDto
            {
                Id = d.Id,
                DonateRequestId = (int)d.DonateRequestId,
                DonatedAt = d.DonatedAt,
                Amount = (decimal)d.DonateRequest.Amount,
                DonatedByUserName = d.DonateRequest.RequestedByUser?.Name ?? "Anonymous"
            });
        }

        public async Task<IEnumerable<DonationReadDto>> GetByUserIdAsync(Guid userId)
        {
            var donations = await _donationRepo.GetByUserIdAsync(userId);
            return donations.Select(d => new DonationReadDto
            {
                Id = d.Id,
                DonateRequestId = (int)d.DonateRequestId,
                DonatedAt = d.DonatedAt,
                Amount = (decimal)d.DonateRequest.Amount,
                DonatedByUserName = d.DonateRequest.RequestedByUser?.Name ?? "Anonymous"
            });
        }

        public async Task<IEnumerable<DonationReadDto>> GetByOrganizationIdAsync(int organizationId)
        {
            var donations = await _donationRepo.GetByOrganizationIdAsync(organizationId);
            return donations.Select(d => new DonationReadDto
            {
                Id = d.Id,
                DonateRequestId = (int)d.DonateRequestId,
                DonatedAt = d.DonatedAt,
                Amount = (decimal)d.DonateRequest.Amount,
                DonatedByUserName = d.DonateRequest.RequestedByUser?.Name ?? "Anonymous"
            });
        }

        public async Task<decimal> GetTotalDonatedAmountAsync()
        {
            var donations = await _donationRepo.GetAllAsync();
            return donations.Sum(d => d.DonateRequest.Amount ?? 0);
        }
    }
}
