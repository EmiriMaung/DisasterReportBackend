using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class ImpactUrl
{
    public int Id { get; set; }

    public int DisasterReportId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? PublicId { get; set; }

    public string? FileType { get; set; }

    public int? FileSizeKb { get; set; }

    public DateTime UploadedAt { get; set; }

    public virtual DisastersReport DisasterReport { get; set; } = null!;
}
