using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class Comment
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public int DisasterReportId { get; set; }

    public string? Content { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual DisastersReport DisasterReport { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
