using DisasterReport.Data.Domain;
using DisasterReport.Data.Dtos;
using DisasterReport.Services.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IActivityService
    {
        Task<ActivityDto> GetActivityByIdAsync(int id);
        Task<List<ActivityDto>> GetAllActivitiesAsync();
        Task<ActivityDto> CreateActivityAsync(CreateActivityDto createDto);
        Task<ActivityDto> UpdateActivityAsync(int id, ActivityDto updateDto);
        Task<bool> DeleteActivityAsync(int id);
        Task<ActivityMediumDto> AddMediaToActivityAsync(int activityId, IFormFile mediaFile);
        Task<bool> RemoveMediaFromActivityAsync(int mediaId);
    }
}
