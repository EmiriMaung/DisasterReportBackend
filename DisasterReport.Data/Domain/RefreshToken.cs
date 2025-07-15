using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class RefreshToken
{
    public int Id { get; set; }

    public string Token { get; set; } = null!;

    public Guid? UserId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? RevokedAt { get; set; }

    public bool IsUsed { get; set; }

    public string? ReplacedByToken { get; set; }

    public virtual User? User { get; set; }
}
