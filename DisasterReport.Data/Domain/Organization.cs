using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class Organization
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string OrganizationEmail { get; set; } = null!;

    public string? Description { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public bool IsBlackListedOrg { get; set; }

    public int Status { get; set; }

    public Guid? ApprovedBy { get; set; }

    public DateTime? ApprovedAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? LogoUrl { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual ICollection<DonateRequest> DonateRequests { get; set; } = new List<DonateRequest>();

    public virtual ICollection<OrganizationDoc> OrganizationDocs { get; set; } = new List<OrganizationDoc>();

    public virtual ICollection<OrganizationMember> OrganizationMembers { get; set; } = new List<OrganizationMember>();
}
