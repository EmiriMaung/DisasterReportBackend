using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class ActivityMedium
{
    public int Id { get; set; }

    public int ActivityId { get; set; }

    public string MediaUrl { get; set; } = null!;

    public string MediaType { get; set; } = null!;

    public DateTime? UploadedAt { get; set; }

    public virtual Activity Activity { get; set; } = null!;
}
