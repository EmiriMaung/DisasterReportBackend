using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class Location
{
    public int Id { get; set; }

    public string? TownshipName { get; set; }

    public string? RegionName { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public virtual ICollection<DisastersReport> DisastersReports { get; set; } = new List<DisastersReport>();
}
