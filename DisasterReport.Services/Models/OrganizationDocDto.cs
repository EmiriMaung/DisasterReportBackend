using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models
{
    public class OrganizationDocDto
    {
        public string ImageUrl { get; set; } = null!;
        public string? FileName { get; set; }
        public string? FileType { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
