using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.ReportDTO
{
    public class ReportCreateDto
    {
        public Guid ReporterId { get; set; }

        public int? ReportedPostId { get; set; }

        public Guid? ReportedUserId { get; set; }

        public string Reason { get; set; } = null!;
    }
}
