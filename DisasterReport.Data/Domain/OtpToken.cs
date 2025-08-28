using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class OtpToken
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string Code { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }
}
