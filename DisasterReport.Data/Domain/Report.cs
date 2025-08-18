using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class Report
{
    public int Id { get; set; }

    public Guid ReporterId { get; set; }

    public int? ReportedPostId { get; set; }

    public Guid? ReportedUserId { get; set; }

    public string Reason { get; set; } = null!;

    public string Status { get; set; } = null!;

    public string? ActionTaken { get; set; }

    public DateTime CreatedAt { get; set; }

    public Guid? ReviewedBy { get; set; }

    public DateTime? ReviewedAt { get; set; }

    public virtual DisastersReport? ReportedPost { get; set; }

    public virtual User? ReportedUser { get; set; }

    public virtual User Reporter { get; set; } = null!;

    public virtual User? ReviewedByNavigation { get; set; }
}
