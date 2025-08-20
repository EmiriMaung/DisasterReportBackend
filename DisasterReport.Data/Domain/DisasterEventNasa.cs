using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class DisasterEventNasa
{
    public int Id { get; set; }

    public string EventId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Category { get; set; }

    public string? SourceUrl { get; set; }

    public DateTime EventDate { get; set; }

    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    public DateTime CreatedAt { get; set; }
}
