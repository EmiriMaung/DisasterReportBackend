using DisasterReport.Services.Enums;

namespace DisasterReport.Services.Models
{
    public class DonateRequestCreateDto
    {
        public string? Description { get; set; }
        public string? SupportType { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentSlipUrl { get; set; }
        public string? FileType { get; set; }
        public int? FileSizeKb { get; set; }

        public int? OrganizationId { get; set; }   // null => platform donation
        public bool IsPlatformDonation { get; set; }
    }

    public class DonateRequestReadDto
    {
        public int Id { get; set; }
        public Guid RequestedByUserId { get; set; }
        public string? RequestedByUserName { get; set; }
        public string? Description { get; set; }
        public string? SupportType { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentSlipUrl { get; set; }
        public int? OrganizationId { get; set; }
        public bool IsPlatformDonation { get; set; }
        public Status Status { get; set; }    
        public DateTime? DonatedAt { get; set; }
    }
    public class DonateRequestReviewDto  //To approve and reject
    {
        public int RequestId { get; set; }
        public bool Accept { get; set; }
    }
}
