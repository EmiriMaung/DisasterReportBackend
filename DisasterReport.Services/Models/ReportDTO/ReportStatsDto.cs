using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.ReportDTO
{
    public class ReportStatsDto
    {
        public int Total { get; set; }
        public int PendingReports { get; set; }
        public int Resolved { get; set; }
        public int Reject { get; set; }
    }
}
