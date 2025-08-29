using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Domain
{
    public class DashboardStatsDto
    {
        public int ActiveUsers { get; set; }
        public int VerifiedReports { get; set; }
        public int ActiveOrganizations { get; set; }
        public decimal TotalPlatformDonations { get; set; }
    }
    public class DailyPlatformDonationDto
    {
        public DateTime DonationDate { get; set; }
        public decimal TotalAmount { get; set; }
    }

}
