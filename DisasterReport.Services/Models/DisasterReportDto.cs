using DisasterReport.Services.Models.UserDTO;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models;

public class DisasterReportDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid ReporterId { get; set; }
    public Guid? UpdateUserId { get; set; }
    public string Category { get; set; } = null!;
    public int LocationId { get; set; }
    public DateTime ReportedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int Status { get; set; }
    public string StatusName { get; set; }
    public bool IsUrgent { get; set; }
    public bool IsDeleted { get; set; }
    public int? DisasterTopicsId { get; set; }

    public LocationDto? Location { get; set; }
    public UserDto? Reporter { get; set; }
    public DisasterTopicDto? DisasterTopic { get; set; }

    public List<ImpactTypeDto> ImpactTypes { get; set; } = new();
    public List<SupportTypeDto> SupportTypes { get; set; } = new();
    public List<ImpactUrlDto> ImpactUrls { get; set; } = new();

}

public class AddDisasterReportDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    //public Guid ReporterId { get; set; }
    public string Category { get; set; } = null!;
    // public int DisasterTopicsId { get; set; }
    public bool IsUrgent { get; set; }

    public LocationDto Location { get; set; } = null!;

    public List<int> ImpactTypeIds { get; set; } = new();
    public List<int> SupportTypeIds { get; set; } = new();

    public List<IFormFile> Files { get; set; } = new();
}
public class UpdateDisasterReportDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string Category { get; set; } = null!;
    public DateTime? UpdateAt { get; set; }
    //public int DisasterTopicsId { get; set; }
    public bool IsUrgent { get; set; }

    public LocationDto Location { get; set; } = null!;

    public List<int> ImpactTypeIds { get; set; } = new();
    public List<int> SupportTypeIds { get; set; } = new();

    public List<IFormFile> Files { get; set; } = new();
}



