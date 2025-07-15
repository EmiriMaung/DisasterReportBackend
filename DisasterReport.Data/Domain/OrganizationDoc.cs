using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class OrganizationDoc
{
    public int Id { get; set; }

    public int OrganizationId { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string? FileName { get; set; }

    public string? FileType { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Organization Organization { get; set; } = null!;
}
