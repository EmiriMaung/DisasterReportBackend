using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class BlacklistEntry
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public Guid CreatedAdminId { get; set; }

    public Guid? UpdatedAdminId { get; set; }

    public string Reason { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdateAt { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User User { get; set; } = null!;
}
