using DisasterReport.Data.Domain;
using DisasterReport.Data.Dtos;
using DisasterReport.Services.Models; 
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Implementations
{
    public class ActivityService : IActivityService
    {
        private readonly ApplicationDBContext _context;
        private readonly ICloudinaryService _cloudStorageService;

        public ActivityService(ApplicationDBContext context, ICloudinaryService cloudStorageService)
        {
            _context = context;
            _cloudStorageService = cloudStorageService;
        }

        public async Task<ActivityDto> GetActivityByIdAsync(int id)
        {
            var activity = await _context.Activities
                .Include(a => a.ActivityMedia)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null) return null;

            return MapToDto(activity);
        }

        public async Task<List<ActivityDto>> GetAllActivitiesAsync()
        {
            var activities = await _context.Activities
                .Include(a => a.ActivityMedia)
                .ToListAsync();

            return activities.Select(MapToDto).ToList();
        }

        public async Task<ActivityDto> CreateActivityAsync(CreateActivityDto createDto)
        {
            var activity = new Activity
            {
                Title = createDto.Title,
                Description = createDto.Description,
                CreatedBy = createDto.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            // Upload media files using custom UploadResult
            if (createDto.MediaFiles != null && createDto.MediaFiles.Any())
            {
                foreach (var mediaFile in createDto.MediaFiles)
                {
                    await AddMediaToActivityAsync(activity.Id, mediaFile);
                }
            }

            return await GetActivityByIdAsync(activity.Id);
        }

        public async Task<ActivityDto> UpdateActivityAsync(int id, ActivityDto updateDto)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return null;

            activity.Title = updateDto.Title;
            activity.Description = updateDto.Description;
            activity.UpdatedAt = DateTime.UtcNow;

            _context.Activities.Update(activity);
            await _context.SaveChangesAsync();

            return await GetActivityByIdAsync(id);
        }

        public async Task<bool> DeleteActivityAsync(int id)
        {
            var activity = await _context.Activities
                .Include(a => a.ActivityMedia)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null) return false;

            // Delete associated media using ICloudinaryService
            var publicIds = activity.ActivityMedia.Select(m => m.MediaUrl).ToList();
            await _cloudStorageService.DeleteFilesAsync(publicIds);

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ActivityMediumDto> AddMediaToActivityAsync(int activityId, IFormFile mediaFile)
        {
            var activity = await _context.Activities.FindAsync(activityId);
            if (activity == null) return null;

            // Upload file using custom UploadResult
            UploadResult uploadResult = await _cloudStorageService.UploadFileAsync(mediaFile);

            var activityMedium = new ActivityMedium
            {
                ActivityId = activityId,
                MediaUrl = uploadResult.SecureUrl, 
                MediaType = GetMediaType(mediaFile.ContentType),
                UploadedAt = DateTime.UtcNow
            };

            _context.ActivityMedia.Add(activityMedium);
            await _context.SaveChangesAsync();

            return new ActivityMediumDto
            {
                Id = activityMedium.Id,
                MediaUrl = activityMedium.MediaUrl,
                MediaType = activityMedium.MediaType,
                UploadedAt = activityMedium.UploadedAt
            };
        }

        public async Task<bool> RemoveMediaFromActivityAsync(int mediaId)
        {
            var media = await _context.ActivityMedia.FindAsync(mediaId);
            if (media == null) return false;

            // Delete media using ICloudinaryService
            await _cloudStorageService.DeleteFilesAsync(new List<string> { media.MediaUrl });

            _context.ActivityMedia.Remove(media);
            await _context.SaveChangesAsync();

            return true;
        }

        private ActivityDto MapToDto(Activity activity)
        {
            return new ActivityDto
            {
                Id = activity.Id,
                Title = activity.Title,
                Description = activity.Description,
                CreatedBy = activity.CreatedBy,
                CreatedAt = activity.CreatedAt,
                UpdatedAt = activity.UpdatedAt,
                Media = activity.ActivityMedia.Select(m => new ActivityMediumDto
                {
                    Id = m.Id,
                    MediaUrl = m.MediaUrl,
                    MediaType = m.MediaType,
                    UploadedAt = m.UploadedAt
                }).ToList()
            };
        }

        private string GetMediaType(string contentType)
        {
            return contentType.ToLower() switch
            {
                var type when type.StartsWith("image/") => "image",
                var type when type.StartsWith("video/") => "video",
                var type when type.StartsWith("audio/") => "audio",
                _ => "document"
            };
        }
    }
}
