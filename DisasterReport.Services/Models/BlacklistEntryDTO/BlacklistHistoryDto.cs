using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.BlacklistEntryDTO
{
    public class BlacklistHistoryDto
    {
        public string EventType { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string? UpdatedReason { get; set; }
        public string AdminName { get; set; } = string.Empty;
    }
}
