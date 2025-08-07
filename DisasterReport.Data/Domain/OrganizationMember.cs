using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class OrganizationMember
{
    public int Id { get; set; }

    public int OrganizationId { get; set; }

    public Guid? UserId { get; set; }

    public string? RoleInOrg { get; set; }

    public DateTime? JoinedAt { get; set; }

    public string? InvitedEmail { get; set; }

    public bool IsAccepted { get; set; }

    public Guid? InvitationToken { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;

    public virtual User? User { get; set; }
}
