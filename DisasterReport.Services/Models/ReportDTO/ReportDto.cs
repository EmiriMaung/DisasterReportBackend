using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.ReportDTO
{
    public class ReportDto
    {
        public int Id { get; set; }

        public Guid ReporterId { get; set; }

        public string ReporterName { get; set; } = null!;

        public Guid? ReportedUserId { get; set; }

        public string? ReportedUserName { get; set; }

        public int? ReportedPostId { get; set; }

        public string Reason { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string? ActionTaken { get; set; }

        public DateTime CreatedAt { get; set; }

        public Guid? ReviewedById { get; set; }

        public string? ReviewedByName { get; set; }

        public DateTime? ReviewedAt { get; set; }
    }
}
