using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace DisasterReport.Data.Dtos
{
    public class ActivityDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<ActivityMediumDto> Media { get; set; } = new List<ActivityMediumDto>();
    }

    public class CreateActivityDto
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int CreatedBy { get; set; }
        public List<IFormFile>? MediaFiles { get; set; }
    }

    public class ActivityMediumDto
    {
        public int Id { get; set; }
        public string MediaUrl { get; set; } = null!;
        public string MediaType { get; set; } = null!;
        public DateTime? UploadedAt { get; set; }
    }
}