using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class ExternalLogin
{
    public int Id { get; set; }

    public Guid UserId { get; set; }

    public string Provider { get; set; } = null!;

    public string ProviderKey { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
