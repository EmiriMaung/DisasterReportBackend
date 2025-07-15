using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class Donation
{
    public int Id { get; set; }

    public int? DonateRequestId { get; set; }

    public DateTime DonatedAt { get; set; }

    public virtual ICollection<Certificate> Certificates { get; set; } = new List<Certificate>();

    public virtual DonateRequest? DonateRequest { get; set; }
}
