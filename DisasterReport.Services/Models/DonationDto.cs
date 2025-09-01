namespace DisasterReport.Services.Models
{
    public class DonationReadDto
    {
        public int Id { get; set; }
        public int DonateRequestId { get; set; }
        public DateTime DonatedAt { get; set; }
        public string DonatedByUserName { get; set; } = default!;
        public decimal Amount { get; set; }
        public string? OrganizationName { get; set; }  // NEW
        public string? SupportType { get; set; }       // optional
    }
    public class OrganizationDonationSummaryDto
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = string.Empty;
        public decimal TotalDonationAmount { get; set; }
        public string Currency { get; set; } = "MMK"; // default MMK
    }
}
