using DisasterReport.Data.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories.Interfaces
{
    public interface IDashboardRepo
    {
        Task<DashboardStatsDto> GetDashboardStatsAsync();
        Task<List<DailyPlatformDonationDto>> GetPlatformDonationsLast30DaysAsync();

    }

}
