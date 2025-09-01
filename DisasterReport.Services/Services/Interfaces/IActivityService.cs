using DisasterReport.Data.Domain;
using DisasterReport.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IActivityService
    {
        // Activities
        // Activities
        Task<List<Activity>> GetAllActivitiesAsync();
        Task<Activity?> GetActivityByIdAsync(int id);
        Task<Activity> CreateActivityAsync(ActivityDto dto);
        Task<bool> UpdateActivityAsync(int id, ActivityDto dto);
        Task<bool> DeleteActivityAsync(int id);

        // Activity Media
        Task<ActivityMedium> AddMediaAsync(ActivityMediumDto dto);
        Task<bool> DeleteMediaAsync(int id);
    }
}
