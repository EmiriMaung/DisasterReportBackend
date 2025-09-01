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
                DonatedByUserName = d.DonateRequest.RequestedByUser?.Name ?? "Anonymous",
                OrganizationName = d.DonateRequest.Organization?.Name ?? "HandsOfHope",  // NEW
                SupportType = d.DonateRequest.SupportType  // optional
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

        public async Task<IEnumerable<OrganizationDonationSummaryDto>> GetOrganizationDonationSummaryAsync()
        {
            var donations = await _donationRepo.GetAllWithOrganizationsAsync();

            var result = donations
                .Where(d => d.DonateRequest?.Organization != null)
                .GroupBy(d => new { d.DonateRequest.OrganizationId, d.DonateRequest.Organization.Name })
                .Select(g => new OrganizationDonationSummaryDto
                {
                    OrganizationId = g.Key.OrganizationId ?? 0,
                    OrganizationName = g.Key.Name,
                    TotalDonationAmount = g.Sum(x => x.DonateRequest?.Amount ?? 0),
                    Currency = "MMK"
                });

            return result;
        }

    }
}
