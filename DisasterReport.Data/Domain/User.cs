namespace DisasterReport.Data.Domain;

public partial class User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int RoleId { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public bool IsBlacklistedUser { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<BlacklistEntry> BlacklistEntries { get; set; } = new List<BlacklistEntry>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<DisasterTopic> DisasterTopics { get; set; } = new List<DisasterTopic>();

    public virtual ICollection<DisastersReport> DisastersReports { get; set; } = new List<DisastersReport>();

    public virtual ICollection<DonateRequest> DonateRequests { get; set; } = new List<DonateRequest>();

    public virtual ICollection<ExternalLogin> ExternalLogins { get; set; } = new List<ExternalLogin>();

    public virtual OrganizationMember? OrganizationMember { get; set; }

    public virtual ICollection<Organization> Organizations { get; set; } = new List<Organization>();

    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

    public virtual UserRole Role { get; set; } = null!;
}
