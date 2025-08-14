namespace DisasterReport.Services.Models
{
    public class OrganizationDocDto
    {
        public string ImageUrl { get; set; } = null!;
        public string? FileName { get; set; }
        public string? FileType { get; set; }// (values: "nrc-front", "nrc-back", "certificate")
        public DateTime CreatedAt { get; set; }
    }
}
