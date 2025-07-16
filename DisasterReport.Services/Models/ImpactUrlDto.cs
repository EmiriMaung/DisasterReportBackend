using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models
{
    public class ImpactUrlDto
    {
        public int Id { get; set; }

        public int DisasterReportId { get; set; }

        public string ImageUrl { get; set; } = null!;

        public string? PublicId { get; set; }

        public string? FileType { get; set; }

        public int? FileSizeKb { get; set; }

        public DateTime UploadedAt { get; set; }

      
    }
}
