using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepo _dashboardRepository;

        public DashboardService(IDashboardRepo dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            return await _dashboardRepository.GetDashboardStatsAsync();
        }
        public async Task<List<DailyPlatformDonationDto>> GetLast30DaysPlatformDonationsAsync()
        {
            return await _dashboardRepository.GetPlatformDonationsLast30DaysAsync();
        }
    }
}
