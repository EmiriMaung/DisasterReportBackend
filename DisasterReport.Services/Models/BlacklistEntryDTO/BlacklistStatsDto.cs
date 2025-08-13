using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.BlacklistEntryDTO
{
    public class BlacklistStatsDto
    {
        public int TotalBlocked { get; set; }
        public int TotalUnlocked { get; set; }
        public int BlockedLast7Days { get; set; }
        public int UnblockedLast7Days { get; set; }
    }
}
