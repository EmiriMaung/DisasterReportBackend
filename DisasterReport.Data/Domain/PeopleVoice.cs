using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Domain;

public partial class PeopleVoice
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
