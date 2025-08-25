namespace DisasterReport.Services.Models
{
    public class DonationReadDto
    {
        public int Id { get; set; }
        public int DonateRequestId { get; set; }
        public DateTime DonatedAt { get; set; }
        public string DonatedByUserName { get; set; } = default!;
        public decimal Amount { get; set; }
    }
}
