using DisasterReport.Data.Domain;
using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Implementations
{
    public class ActivityService : IActivityService
    {
        private readonly ApplicationDBContext _context;

        public ActivityService(ApplicationDBContext context)
        {
            _context = context;
        }

        // Get all activities with media
        public async Task<List<Activity>> GetAllActivitiesAsync()
        {
            return await _context.Activities
                .Include(a => a.ActivityMedia)
                .ToListAsync();
        }

        // Get activity by id
        public async Task<Activity?> GetActivityByIdAsync(int id)
        {
            return await _context.Activities
                .Include(a => a.ActivityMedia)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // Create activity
        public async Task<Activity> CreateActivityAsync(ActivityDto dto)
        {
            var activity = new Activity
            {
                Title = dto.Title,
                Description = dto.Description,
                CreatedBy = dto.CreatedBy,
                CreatedAt = DateTime.UtcNow
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();
            return activity;
        }

        // Update activity
        public async Task<bool> UpdateActivityAsync(int id, ActivityDto dto)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return false;

            activity.Title = dto.Title;
            activity.Description = dto.Description;
            activity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        // Delete activity
        public async Task<bool> DeleteActivityAsync(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return false;

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();
            return true;
        }

        // Add media to activity
        public async Task<ActivityMedium> AddMediaAsync(ActivityMediumDto dto)
        {
            var media = new ActivityMedium
            {
                ActivityId = dto.ActivityId,
                MediaUrl = dto.MediaUrl,
                MediaType = dto.MediaType,
                UploadedAt = DateTime.UtcNow
            };

            _context.ActivityMedia.Add(media);
            await _context.SaveChangesAsync();
            return media;
        }

        // Delete media
        public async Task<bool> DeleteMediaAsync(int id)
        {
            var media = await _context.ActivityMedia.FindAsync(id);
            if (media == null) return false;

            _context.ActivityMedia.Remove(media);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
