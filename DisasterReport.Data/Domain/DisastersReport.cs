using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class DisastersReport
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public Guid ReporterId { get; set; }

    public Guid? UpdateUserId { get; set; }

    public string Category { get; set; } = null!;

    public int LocationId { get; set; }

    public DateTime ReportedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int Status { get; set; }

    public bool IsUrgent { get; set; }

    public bool IsDeleted { get; set; }

    public int? DisasterTopicsId { get; set; }

    public Guid? ProcessedBy { get; set; }

    public DateTime? ProcessedAt { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual DisasterTopic? DisasterTopics { get; set; }

    public virtual ICollection<ImpactUrl> ImpactUrls { get; set; } = new List<ImpactUrl>();

    public virtual Location Location { get; set; } = null!;

    public virtual User Reporter { get; set; } = null!;

    public virtual ICollection<ImpactType> ImpactTypes { get; set; } = new List<ImpactType>();

    public virtual ICollection<SupportType> SupportTypes { get; set; } = new List<SupportType>();
}
