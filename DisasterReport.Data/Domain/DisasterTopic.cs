using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class DisasterTopic
{
    public int Id { get; set; }

    public Guid? AdminId { get; set; }

    public Guid? UpdatedAdminId { get; set; }

    public string? TopicName { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdateAt { get; set; }

    public virtual User? Admin { get; set; }

    public virtual ICollection<DisastersReport> DisastersReports { get; set; } = new List<DisastersReport>();
}
