using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class SupportType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<DisastersReport> DisasterReports { get; set; } = new List<DisastersReport>();
}
