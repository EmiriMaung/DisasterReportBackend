using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories.Implementations
{
    public class DashboardRepo : IDashboardRepo
    {
        private readonly ApplicationDBContext _context;
        public DashboardRepo(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task<DashboardStatsDto> GetDashboardStatsAsync()
        {
            DashboardStatsDto result = new DashboardStatsDto();
            var connection = _context.Database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                await connection.OpenAsync();
            using (var command = connection.CreateCommand())
            {
                command.CommandText = "sp_GetDashboardCardStats";
                command.CommandType = System.Data.CommandType.StoredProcedure;
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        result.ActiveUsers = reader.GetInt32(reader.GetOrdinal("ActiveUsers"));
                        result.VerifiedReports = reader.GetInt32(reader.GetOrdinal("VerifiedReports"));
                        result.ActiveOrganizations = reader.GetInt32(reader.GetOrdinal("ActiveOrganizations"));
                        result.TotalPlatformDonations = reader.GetDecimal(reader.GetOrdinal("TotalPlatformDonations"));
                    }
                }
            }
            return result;
        }
        public async Task<List<DailyPlatformDonationDto>> GetPlatformDonationsLast30DaysAsync()
        {
            // Call stored procedure
            return await _context.DailyPlatformDonationDtos
                .FromSqlRaw("EXEC dbo.GetPlatformDonationsLast30Days")
                .ToListAsync();
        }
    }
  
}