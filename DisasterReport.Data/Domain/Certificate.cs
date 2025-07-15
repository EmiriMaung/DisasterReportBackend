using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class Certificate
{
    public int Id { get; set; }

    public int DonationId { get; set; }

    public string? Title { get; set; }

    public string? SupportType { get; set; }

    public DateOnly? IssueDate { get; set; }

    public string? DonarName { get; set; }

    public decimal? Amount { get; set; }

    public virtual Donation Donation { get; set; } = null!;
}
