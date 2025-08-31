using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class Activity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<ActivityMedium> ActivityMedia { get; set; } = new List<ActivityMedium>();
}
