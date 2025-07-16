using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Enums;
using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace DisasterReport.Services.Services.Implementations
{
    public class DisasterReportService : IDisasterReportService
    {
        private readonly IPostRepo _postRepo;
        private readonly ILocationRepo _locationRepo;
        private readonly IImpactUrlRepo _impactUrlRepo;
        private readonly ICloudinaryService _cloudinaryService;


        private readonly IMemoryCache _cache;

        public DisasterReportService(
            IPostRepo postRepo,
            ICloudinaryService cloudinaryService,
            IImpactUrlRepo impactUrlRepo,
            ILocationRepo locationRepo,
            IMemoryCache memoryCache)
        {
            _postRepo = postRepo;
            _cloudinaryService = cloudinaryService;
            _impactUrlRepo = impactUrlRepo;
            _locationRepo = locationRepo;
            _cache = memoryCache;
        }

        public async Task<IEnumerable<DisasterReportDto>> GetAllReportsAsync()
        {
            const string cacheKey = "all_reports";

            if (_cache.TryGetValue(cacheKey, out List<DisasterReportDto> cachedReports))
            {
                return cachedReports;
            }

            var reports = await _postRepo.GetAllPostsWithMaterialsAsync();
            var dtoList = await MapToDtoListAsync(reports);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)); 

            _cache.Set(cacheKey, dtoList, cacheOptions);

            return dtoList;
        }
        public async Task<IEnumerable<DisasterReportDto>> GetUrgentReportsAsync()
        {
            const string cacheKey = "all_reports";

            if (_cache.TryGetValue(cacheKey, out List<DisasterReportDto> cachedReports))
            {
                return cachedReports;
            }

            var reports = await _postRepo.GetUrgentReportsAsync();
            var dtoList = await MapToDtoListAsync(reports);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)); 

            _cache.Set(cacheKey, dtoList, cacheOptions);

            return dtoList;
        }

        public async Task<IEnumerable<DisasterReportDto>> GetAllReportsByReporterIdAsync(Guid reporterId)
        {
            var cacheKey = $"reports_by_reporter_{reporterId}";

            if (_cache.TryGetValue(cacheKey, out List<DisasterReportDto> cachedReports))
            {
                return cachedReports;
            }

            var reports = await _postRepo.GetAllPostsByReporterId(reporterId);
            var dtoList = await MapToDtoListAsync(reports);

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)); 

            _cache.Set(cacheKey, dtoList, cacheOptions);

            return dtoList;
        }

        public async Task<IEnumerable<DisasterReportDto>> GetDeletedReportsByReporterIdAsync(Guid reporterId)
        {
            var reports = await _postRepo.GetDeletedPostsByReporterId(reporterId);
            return await MapToDtoListAsync(reports);
        }

        public async Task<IEnumerable<DisasterReportDto>> GetDeletedReportsAsync(string category)
        {
            throw new NotImplementedException("This method is not implemented yet.");
        }

        public async Task<DisasterReportDto?> GetReportByIdAsync(int id)
        {
            var cacheKey = $"report_{id}";

            if (_cache.TryGetValue(cacheKey, out DisasterReportDto cachedReport))
            {
                return cachedReport;
            }

            var report = await _postRepo.GetPostByIdAsync(id);
            if (report == null) return null;

            var dtoList = await MapToDtoListAsync(new List<DisastersReport> { report });
            var dto = dtoList.FirstOrDefault();

            if (dto != null)
            {
                _cache.Set(cacheKey, dto, TimeSpan.FromMinutes(10));
            }

            return dto;
        }
        public async Task<IEnumerable<DisasterReportDto>> GetReportsByRegionAsync(string regionName)
        {
            var reports = await _postRepo.GetReportsByRegionAsync(regionName);
            return await MapToDtoListAsync(reports);
        }

        public async Task<IEnumerable<DisasterReportDto>> GetReportsByTownshipAsync(string townshipName)
        {
            var reports = await _postRepo.GetReportsByTownshipAsync(townshipName);
            return await MapToDtoListAsync(reports);
        }
        public async Task<IEnumerable<DisasterReportDto>> GetReportsByStatusAsync(int status)
        {
            var reports = await _postRepo.GetReportsByStatusAsync(status);
            return await MapToDtoListAsync(reports);
        }
        public async Task AddReportAsync(AddDisasterReportDto report)
        {
            var uploadedFiles = new List<ImpactUrlDto>();

            await using var transaction = await _postRepo.DbContext.Database.BeginTransactionAsync();

            try
            {
                var newLocation = new Location
                {
                    TownshipName = report.Location.TownshipName,
                    RegionName = report.Location.RegionName,
                    Latitude = report.Location.Latitude,
                    Longitude = report.Location.Longitude
                };

                await _locationRepo.AddAsync(newLocation);
                await _locationRepo.SaveChangesAsync();

                uploadedFiles = await _cloudinaryService.UploadFilesAsync(report.Files);

                var newReport = new DisastersReport
                {
                    Title = report.Title,
                    Description = report.Description,
                    Category = report.Category,
                    ReporterId = report.ReporterId,
                    LocationId = newLocation.Id,
                    // DisasterTopicsId = report.DisasterTopicsId,
                    IsUrgent = report.IsUrgent,
                    IsDeleted = false,
                    ReportedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    ImpactUrls = uploadedFiles.Select(f => new ImpactUrl
                    {
                        ImageUrl = f.ImageUrl,
                        PublicId = f.PublicId,
                        FileType = f.FileType,
                        FileSizeKb = f.FileSizeKb,
                        UploadedAt = f.UploadedAt
                    }).ToList()
                };

                if (report.ImpactTypeIds?.Any() == true)
                {
                    newReport.ImpactTypes = await _postRepo.DbContext.ImpactTypes
                        .Where(i => report.ImpactTypeIds.Contains(i.Id))
                        .ToListAsync();
                }

                if (report.SupportTypeIds?.Any() == true)
                {
                    newReport.SupportTypes = await _postRepo.DbContext.SupportTypes
                        .Where(s => report.SupportTypeIds.Contains(s.Id))
                        .ToListAsync();
                }

                await _postRepo.AddPostAsync(newReport);
                await _postRepo.SaveChangesAsync();

                await transaction.CommitAsync();
                _cache.Remove($"reports_by_reporter_{report.ReporterId}");

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                if (uploadedFiles.Any())
                {
                    var publicIds = uploadedFiles.Select(f => f.PublicId).ToList();
                    await _cloudinaryService.DeleteFilesAsync(publicIds);
                }

                throw new Exception("Failed to add disaster report: " + ex.Message, ex);
            }
        }

        public async Task UpdateReportAsync(int reportId, UpdateDisasterReportDto reportDto)
        {
            var uploadedFiles = new List<ImpactUrlDto>();

            await using var transaction = await _postRepo.DbContext.Database.BeginTransactionAsync();

            try
            {
                var report = await _postRepo.GetPostByIdAsync(reportId);
                if (report == null)
                    throw new Exception("Report not found.");

                if (report.ImpactUrls?.Any() == true)
                {
                    foreach (var impactUrl in report.ImpactUrls)
                    {
                        await _impactUrlRepo.DeleteAsync(impactUrl.Id);
                    }
                    await _impactUrlRepo.SaveChangesAsync();
                }


                uploadedFiles = await _cloudinaryService.UploadFilesAsync(reportDto.Files);

                var existingLocation = await _locationRepo.GetByIdAsync(report.LocationId);
                if (existingLocation == null)
                    throw new Exception("Existing location not found.");

                existingLocation.TownshipName = reportDto.Location.TownshipName;
                existingLocation.RegionName = reportDto.Location.RegionName;
                existingLocation.Latitude = reportDto.Location.Latitude;
                existingLocation.Longitude = reportDto.Location.Longitude;

                await _locationRepo.UpdateAsync(existingLocation);
                await _locationRepo.SaveChangesAsync();

                report.Title = reportDto.Title;
                report.Description = reportDto.Description;
                report.Category = reportDto.Category;
                // report.DisasterTopicsId = reportDto.DisasterTopicsId;
                report.IsUrgent = reportDto.IsUrgent;
                report.UpdatedAt = DateTime.UtcNow;
                report.LocationId = existingLocation.Id;

                report.ImpactUrls = uploadedFiles.Select(f => new ImpactUrl
                {
                    ImageUrl = f.ImageUrl,
                    PublicId = f.PublicId,
                    FileType = f.FileType,
                    FileSizeKb = f.FileSizeKb,
                    UploadedAt = f.UploadedAt
                }).ToList();

                if (reportDto.ImpactTypeIds?.Any() == true)
                {
                    report.ImpactTypes = await _postRepo.DbContext.ImpactTypes
                        .Where(i => reportDto.ImpactTypeIds.Contains(i.Id))
                        .ToListAsync();
                }

                if (reportDto.SupportTypeIds?.Any() == true)
                {
                    report.SupportTypes = await _postRepo.DbContext.SupportTypes
                        .Where(s => reportDto.SupportTypeIds.Contains(s.Id))
                        .ToListAsync();
                }

                await _postRepo.UpdatePostAsync(report);
                await _postRepo.SaveChangesAsync();

                await transaction.CommitAsync();
                _cache.Remove($"reports_by_reporter_{report.ReporterId}");

            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                if (uploadedFiles.Any())
                {
                    var publicIds = uploadedFiles.Select(f => f.PublicId).ToList();
                    await _cloudinaryService.DeleteFilesAsync(publicIds);
                }

                throw new Exception("Failed to update disaster report: " + ex.Message, ex);
            }
        }

        public async Task SoftDeleteAsync(int id)
        {
            var report = await _postRepo.GetPostByIdAsync(id);
            if (report == null) throw new Exception("Report not found.");

            await _postRepo.SoftDeleteReportAsync(id);
            await _postRepo.SaveChangesAsync();
            _cache.Remove($"reports_by_reporter_{report.ReporterId}");

        }

        public async Task RestoreReportAsync(int id)
        {
            var report = await _postRepo.GetPostByIdAsync(id);
            if (report == null) throw new Exception("Report not found.");

            await _postRepo.RestoreDeletedReportAsync(id);
            report.UpdatedAt = DateTime.UtcNow;
            await _postRepo.SaveChangesAsync();
            _cache.Remove($"reports_by_reporter_{report.ReporterId}");

        }

        public async Task HardDeleteAsync(int id)
        {
            await using var transaction = await _postRepo.DbContext.Database.BeginTransactionAsync();
            try
            {
                var report = await _postRepo.GetPostByIdAsync(id);
                if (report == null)
                    throw new Exception("Report not found.");

                if (report.ImpactUrls?.Any() == true)
                {
                    var publicIds = report.ImpactUrls.Select(f => f.PublicId).ToList();
                    await _cloudinaryService.DeleteFilesAsync(publicIds);
                }

                if (report.ImpactUrls?.Any() == true)
                {
                    foreach (var impactUrl in report.ImpactUrls)
                    {
                        await _impactUrlRepo.DeleteAsync(impactUrl.Id);
                    }
                    await _impactUrlRepo.SaveChangesAsync();
                }

                // Clear many-to-many relationships
                report.ImpactTypes?.Clear();
                report.SupportTypes?.Clear();

                var locationId = report.LocationId;

                // Delete the report itself
                await _postRepo.HardDeleteReportAsync(report);
                await _postRepo.SaveChangesAsync();

                // Delete the location if no other reports use it
                var otherReportsUseLocation = await _postRepo.DbContext.DisastersReports
                    .AnyAsync(r => r.LocationId == locationId);
                if (!otherReportsUseLocation)
                {
                    var location = await _locationRepo.GetByIdAsync(locationId);
                    if (location != null)
                        await _locationRepo.DeleteAsync(locationId);
                }

                await _postRepo.SaveChangesAsync();
                await transaction.CommitAsync();
                _cache.Remove($"reports_by_reporter_{report.ReporterId}");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to hard delete report: " + ex.Message, ex);
            }
        }

        public async Task<IEnumerable<DisasterReportDto>> SearchReportsAsync(string? keyword, string? category, string? region, bool? isUrgent)
        {
            var reports = await _postRepo.SearchReportsAsync(keyword, category, region, isUrgent);
            return await MapToDtoListAsync(reports);
        } 

        public async Task ApproveReportAsync(int reportId, Guid approvedBy)
        {
            await _postRepo.ApproveReportAsync(reportId, approvedBy);
            await _postRepo.SaveChangesAsync();

            _cache.Remove($"report_{reportId}");

            _cache.Remove("all_reports");

        }

        public async Task RejectReportAsync(int reportId, Guid rejectedBy)
        {
            await _postRepo.RejectReportAsync(reportId, rejectedBy);
            await _postRepo.SaveChangesAsync();

            _cache.Remove($"report_{reportId}");

            _cache.Remove("all_reports");
        }
        private async Task<List<DisasterReportDto>> MapToDtoListAsync(List<DisastersReport> reports)
        {
            return reports.Select(report => new DisasterReportDto
            {
                Id = report.Id,
                Title = report.Title,
                Description = report.Description,
                Category = report.Category,
                ReporterId = report.ReporterId,
                UpdatedAt = report.UpdatedAt,
                ReportedAt = report.ReportedAt,
                LocationId = report.LocationId,
                DisasterTopicsId = report.DisasterTopicsId,
                Status = report.Status,
                StatusName = EnumHelper.GetStatusName((int)report.Status),
                IsUrgent = report.IsUrgent,
                IsDeleted = report.IsDeleted,

                Location = report.Location == null ? null : new LocationDto
                {
                    Id = report.Location.Id,
                    TownshipName = report.Location.TownshipName,
                    RegionName = report.Location.RegionName,
                    Latitude = (decimal)report.Location.Latitude,
                    Longitude = (decimal)report.Location.Longitude
                },

                DisasterTopic = report.DisasterTopics == null ? null : new DisasterTopicDto
                {
                    Id = report.DisasterTopics.Id,
                    TopicName = report.DisasterTopics.TopicName
                },

                Reporter = report.Reporter == null ? null : new UserDto
                {
                    Id = report.Reporter.Id,
                    Name = report.Reporter.Name,
                    Email = report.Reporter.Email
                },

                ImpactUrls = report.ImpactUrls?.Select(i => new ImpactUrlDto
                {
                    Id = i.Id,
                    DisasterReportId = i.DisasterReportId,
                    ImageUrl = i.ImageUrl,
                    FileType = i.FileType,
                    PublicId = i.PublicId,
                    FileSizeKb = i.FileSizeKb,
                    UploadedAt = i.UploadedAt
                }).ToList() ?? new List<ImpactUrlDto>(),

                ImpactTypes = report.ImpactTypes?.Select(i => new ImpactTypeDto
                {
                    Id = i.Id,
                    Name = i.Name
                }).ToList() ?? new List<ImpactTypeDto>(),

                SupportTypes = report.SupportTypes?.Select(s => new SupportTypeDto
                {
                    Id = s.Id,
                    Name = s.Name
                }).ToList() ?? new List<SupportTypeDto>()

            }).ToList();
        }
    }
}
