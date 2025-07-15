using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class DonateRequest
{
    public int Id { get; set; }

    public Guid RequestedByUserId { get; set; }

    public int DisasterReportId { get; set; }

    public string? Description { get; set; }

    public string? SupportType { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentSlipUrl { get; set; }

    public string? FileType { get; set; }

    public int? FileSizeKb { get; set; }

    public int Status { get; set; }

    public DateTime DonatedAt { get; set; }

    public virtual DisastersReport DisasterReport { get; set; } = null!;

    public virtual ICollection<Donation> Donations { get; set; } = new List<Donation>();

    public virtual User RequestedByUser { get; set; } = null!;
}
