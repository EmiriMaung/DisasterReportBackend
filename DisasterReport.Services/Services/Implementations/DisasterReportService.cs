using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Enums;
using DisasterReport.Services.Models;
using DisasterReport.Services.Models.Common;
using DisasterReport.Services.Models.UserDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;

namespace DisasterReport.Services.Services.Implementations
{
    public class DisasterReportService : IDisasterReportService
    {
        private readonly IPostRepo _postRepo;
        private readonly ILocationRepo _locationRepo;
        private readonly IImpactUrlRepo _impactUrlRepo;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IDisasterTopicService _disasterTopicService;


        private readonly IMemoryCache _cache;

        public DisasterReportService(
            IPostRepo postRepo,
            ICloudinaryService cloudinaryService,
            IImpactUrlRepo impactUrlRepo,
            ILocationRepo locationRepo,
            IMemoryCache memoryCache,
            IDisasterTopicService disasterTopicService)
        {
            _postRepo = postRepo;
            _cloudinaryService = cloudinaryService;
            _impactUrlRepo = impactUrlRepo;
            _locationRepo = locationRepo;
            _cache = memoryCache;
            _disasterTopicService = disasterTopicService;
        }

        public async Task<PagedResponse<DisasterReportDto>> GetAllReportsAsync(int pageNumber = 1, int pageSize = 10)
        {
            //string cacheKey = $"all_reports_{pageNumber}_{pageSize}";

            //if (_cache.TryGetValue(cacheKey, out PagedResponse<DisasterReportDto> cachedResponse))
            //{
            //    return cachedResponse;
            //}

            var reports = await _postRepo.GetAllPostsWithMaterialsAsync();
            var totalRecords = reports.Count();
            var pagedReports = reports
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoList = await MapToDtoListAsync(pagedReports);

            var response = new PagedResponse<DisasterReportDto>(
                dtoList,
                pageNumber,
                pageSize,
                totalRecords
            );

            //var cacheOptions = new MemoryCacheEntryOptions()
            //    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            //_cache.Set(cacheKey, response, cacheOptions);

            return response;
        }
        public async Task<PagedResponse<DisasterReportDto>> SearchReportsAsync(
            string? keyword,
            string? category,
            string? region,
            string? township,
            bool? isUrgent,
            int? topicId,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var reports = await _postRepo.SearchReportsAsync(keyword, category, region, township, isUrgent, topicId);
            var totalRecords = reports.Count();
            var pagedReports = reports
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoList = await MapToDtoListAsync(pagedReports);

            return new PagedResponse<DisasterReportDto>(
                dtoList,
                pageNumber,
                pageSize,
                totalRecords
            );
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
    

        public async Task<IEnumerable<DisasterReportDto>> GetPendingRejectReportByReporterIdAsync(Guid reporterId)
        {
            var cacheKey = $"pending_reject_reports_by_reporter_{reporterId}";

            if (_cache.TryGetValue(cacheKey, out List<DisasterReportDto> cachedReports))
            {
                return cachedReports;
            }

            var reports = await _postRepo.GetPendingRejectReportByReporterIdAsync(reporterId);
            var dtoList = await MapToDtoListAsync(reports);

            _cache.Set(cacheKey, dtoList, new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10)));

            return dtoList;
        }

        public async Task<IEnumerable<DisasterReportDto>> GetDeletedReportsByReporterIdAsync(Guid reporterId)
        {
            var reports = await _postRepo.GetDeletedPostsByReporterId(reporterId);
            return await MapToDtoListAsync(reports);
        }
        public async Task<IEnumerable<DisasterReportDto>> GetMyReportsAsync(Guid reporterId)
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

        public async Task<IEnumerable<DisasterReportDto>> GetMyDeletedReportsAsync(Guid reporterId)
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

        public async Task<ReportStatusCountDto> CountReportsByStatusAsync()
        {
            var counts = await _postRepo.GetReportCountsByStatusAsync();

            return new ReportStatusCountDto
            {
                Total = counts.total,
                Approve = counts.approve,
                Pending = counts.pending,
                Reject = counts.reject
            };
        }
        public async Task<PagedResponse<DisasterReportDto>> GetReportsByStatusAsync(int? status, int pageNumber = 1, int pageSize = 18)
        {
            var allReports = await _postRepo.GetReportsByStatusAsync(status);

            var totalRecords = allReports.Count;
            var pagedData = allReports
                .OrderByDescending(r => r.ReportedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var reportDtos = await MapToDtoListAsync(pagedData);

            return new PagedResponse<DisasterReportDto>(
                reportDtos,
                pageNumber,
                pageSize,
                totalRecords);
        }
        public async Task AddReportAsync(AddDisasterReportDto report,Guid reporterId)
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
                    ReporterId = reporterId,
                    LocationId = newLocation.Id,
                    // DisasterTopicsId = report.DisasterTopicsId,
                    IsUrgent = report.IsUrgent,
                    IsDeleted = false,
                    ReportedAt = DateTime.Now,
                    //UpdatedAt = DateTime.UtcNow,
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
                ClearReportCache(0, reporterId); 


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
                report.UpdatedAt = reportDto.UpdateAt ?? DateTime.Now;
                report.IsUrgent = reportDto.IsUrgent;
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
                ClearReportCache(report.Id, report.ReporterId);

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
            ClearReportCache(report.Id, report.ReporterId);

        }

        public async Task RestoreReportAsync(int id)
        {
            var report = await _postRepo.GetPostByIdAsync(id);
            if (report == null) throw new Exception("Report not found.");

            await _postRepo.RestoreDeletedReportAsync(id);
            report.UpdatedAt = DateTime.UtcNow;
            await _postRepo.SaveChangesAsync();
            ClearReportCache(report.Id, report.ReporterId);

        }

        //public async Task HardDeleteAsync(int id)
        //{
        //    await using var transaction = await _postRepo.DbContext.Database.BeginTransactionAsync();
        //    try
        //    {
        //        var report = await _postRepo.GetPostByIdAsync(id);
        //        if (report == null)
        //            throw new Exception("Report not found.");

        //        if (report.ImpactUrls?.Any() == true)
        //        {
        //            var publicIds = report.ImpactUrls.Select(f => f.PublicId).ToList();
        //            await _cloudinaryService.DeleteFilesAsync(publicIds);
        //        }

        //        if (report.ImpactUrls?.Any() == true)
        //        {
        //            foreach (var impactUrl in report.ImpactUrls)
        //            {
        //                await _impactUrlRepo.DeleteAsync(impactUrl.Id);
        //            }
        //            await _impactUrlRepo.SaveChangesAsync();
        //        }

        //        // Clear many-to-many relationships
        //        report.ImpactTypes?.Clear();
        //        report.SupportTypes?.Clear();

        //        var locationId = report.LocationId;

        //        // Delete the report itself
        //        await _postRepo.HardDeleteReportAsync(report);
        //        await _postRepo.SaveChangesAsync();

        //        // Delete the location if no other reports use it
        //        var otherReportsUseLocation = await _postRepo.DbContext.DisastersReports
        //            .AnyAsync(r => r.LocationId == locationId);
        //        if (!otherReportsUseLocation)
        //        {
        //            var location = await _locationRepo.GetByIdAsync(locationId);
        //            if (location != null)
        //                await _locationRepo.DeleteAsync(locationId);
        //        }

        //        await _postRepo.SaveChangesAsync();
        //        await transaction.CommitAsync();
        //        ClearReportCache(report.Id, report.ReporterId);
        //    }
        //    catch (Exception ex)
        //    {
        //        await transaction.RollbackAsync();
        //        throw new Exception("Failed to hard delete report: " + ex.Message, ex);
        //    }
        //}
        public async Task HardDeleteAsync(int id)
        {
            await using var transaction = await _postRepo.DbContext.Database.BeginTransactionAsync();
            try
            {
                var report = await _postRepo.GetPostByIdAsync(id);
                if (report == null)
                    throw new Exception("Report not found.");

                // 1️⃣ Delete Impact URLs from Cloudinary
                if (report.ImpactUrls?.Any() == true)
                {
                    var publicIds = report.ImpactUrls.Select(f => f.PublicId).ToList();
                    await _cloudinaryService.DeleteFilesAsync(publicIds);

                    foreach (var impactUrl in report.ImpactUrls)
                    {
                        await _impactUrlRepo.DeleteAsync(impactUrl.Id);
                    }
                    await _impactUrlRepo.SaveChangesAsync();
                }

                // 2️⃣ Delete comments related to this report
                var comments = await _postRepo.DbContext.Comments
                    .Where(c => c.DisasterReportId == id)
                    .ToListAsync();

                if (comments.Any())
                {
                    _postRepo.DbContext.Comments.RemoveRange(comments);
                    await _postRepo.DbContext.SaveChangesAsync();
                }

                // 3️⃣ Clear many-to-many relationships
                report.ImpactTypes?.Clear();
                report.SupportTypes?.Clear();

                var locationId = report.LocationId;

                // 4️⃣ Delete the report itself
                await _postRepo.HardDeleteReportAsync(report);
                await _postRepo.SaveChangesAsync();

                // 5️⃣ Delete location if unused
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

                // 6️⃣ Clear cache
                ClearReportCache(report.Id, report.ReporterId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to hard delete report: " + ex.Message, ex);
            }
        }


        public async Task<PagedResponse<DisasterReportDto>> GetReportsByOrganizationIdAsync(int organizationId, int pageNumber = 1, int pageSize = 10)
        {
            var reports = await _postRepo.GetReportsByOrganizationIdAsync(organizationId);
            var totalRecords = reports.Count;
            var pagedReports = reports
                .OrderByDescending(r => r.ReportedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var dtoList = await MapToDtoListAsync(pagedReports);

            return new PagedResponse<DisasterReportDto>(
                dtoList,
                pageNumber,
                pageSize,
                totalRecords
            );
        }

        public async Task ApproveReportAsync(int reportId, ApproveWithTopicDto topicDto, Guid adminId)
        {
            using var transaction = await _postRepo.DbContext.Database.BeginTransactionAsync();

            try
            {
                var report = await _postRepo.GetPostByIdAsync(reportId);
                if (report == null) throw new Exception("Report not found");

                int topicId;

                if (topicDto.ExistingTopicId.HasValue)
                {
                    // Check if the topic exists
                    var existingTopic = await _postRepo.DbContext.DisasterTopics
                        .FirstOrDefaultAsync(t => t.Id == topicDto.ExistingTopicId.Value);

                    if (existingTopic != null)
                    {
                        topicId = existingTopic.Id;
                    }
                    else if (topicDto.NewTopic != null)
                    {
                        var newTopic = new DisasterTopic
                        {
                            TopicName = topicDto.NewTopic.TopicName,
                            AdminId = adminId, // Use the passed adminId here
                            CreatedAt = DateTime.UtcNow,
                            UpdateAt = DateTime.UtcNow
                        };

                        _postRepo.DbContext.DisasterTopics.Add(newTopic);
                        await _postRepo.DbContext.SaveChangesAsync();

                        topicId = newTopic.Id;
                    }
                    else
                    {
                        throw new ArgumentException("Topic ID not found and new topic info is missing.");
                    }
                }
                else if (topicDto.NewTopic != null)
                {
                    var existingTopic = await _postRepo.DbContext.DisasterTopics
                        .FirstOrDefaultAsync(t => t.TopicName.ToLower() == topicDto.NewTopic.TopicName.ToLower());

                    if (existingTopic != null)
                    {
                        topicId = existingTopic.Id;
                    }
                    else
                    {
                        var newTopic = new DisasterTopic
                        {
                            TopicName = topicDto.NewTopic.TopicName,
                            AdminId = adminId, // Use the passed adminId here
                            CreatedAt = DateTime.UtcNow,
                            UpdateAt = DateTime.UtcNow
                        };

                        _postRepo.DbContext.DisasterTopics.Add(newTopic);
                        await _postRepo.DbContext.SaveChangesAsync();

                        topicId = newTopic.Id;
                    }
                }
                else
                {
                    throw new ArgumentException("Must provide either existing topic ID or new topic details.");
                }

                // Update the report with processing information
                report.Status = 1;
                report.UpdatedAt = DateTime.UtcNow;
                report.DisasterTopicsId = topicId;
                report.ProcessedBy = adminId; // Set the admin who processed this report
                report.ProcessedAt = DateTime.UtcNow; // Set the processing time

                await _postRepo.UpdatePostAsync(report);
                await _postRepo.SaveChangesAsync();

                await transaction.CommitAsync();

                ClearReportCache(report.Id, report.ReporterId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to approve report. Transaction rolled back.", ex);
            }
        }
      
        public async Task RejectReportAsync(int reportId, Guid rejectedBy)
        {
            using var transaction = await _postRepo.DbContext.Database.BeginTransactionAsync();

            try
            {
                var report = await _postRepo.GetPostByIdAsync(reportId);
                if (report == null)
                {
                    throw new ArgumentException("Report not found");
                }

                // Update report status and rejection info
                report.Status = 2; // Assuming 2 means "Rejected"
                report.UpdatedAt = DateTime.UtcNow;
                report.ProcessedBy = rejectedBy;
                report.ProcessedAt = DateTime.UtcNow;

                await _postRepo.UpdatePostAsync(report);
                await _postRepo.SaveChangesAsync();
                await transaction.CommitAsync();

                ClearReportCache(report.Id, report.ReporterId);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Failed to reject report. Transaction rolled back.", ex);
            }
        }
      
        private async Task<List<DisasterReportDto>> MapToDtoListAsync(List<DisastersReport> reports)
        {
            // Reporter ID list ထုတ်ယူ
            var reporterIds = reports
                .Where(r => r.ReporterId != Guid.Empty)
                .Select(r => r.ReporterId)
                .Distinct()
                .ToList();

            // Processor ID list ထုတ်ယူ
            var processorIds = reports
                .Where(r => r.ProcessedBy != null)
                .Select(r => (Guid)r.ProcessedBy)
                .Distinct()
                .ToList();

            // Org member တွေ fetch (for both reporters and processors)
            var allUserIds = reporterIds.Concat(processorIds).Distinct().ToList();
            var orgMembers = await _postRepo.DbContext.OrganizationMembers
                .Include(m => m.Organization)
                .ThenInclude(o => o.OrganizationDocs)
                .Where(m => allUserIds.Contains((Guid)m.UserId))
                .ToListAsync();

            // Fetch processor users
            var processors = processorIds.Any()
                ? await _postRepo.DbContext.Users
                    .Where(u => processorIds.Contains(u.Id))
                    .ToListAsync()
                : new List<User>();

            return reports.Select(report =>
            {
                var orgMember = orgMembers.FirstOrDefault(m => m.UserId == report.ReporterId);
                var processor = processors.FirstOrDefault(u => u.Id == report.ProcessedBy);
                var processorOrgMember = report.ProcessedBy != null
                    ? orgMembers.FirstOrDefault(m => m.UserId == report.ProcessedBy)
                    : null;

                return new DisasterReportDto
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
                    ProcessedBy = report.ProcessedBy,
                    ProcessedAt = report.ProcessedAt,

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
                        Email = report.Reporter.Email,
                        ProfilePictureUrl = report.Reporter.ProfilePictureUrl
                    },

                    Processor = processor == null ? null : new UserDto
                    {
                        Id = processor.Id,
                        Name = processor.Name,
                        Email = processor.Email,
                        ProfilePictureUrl = processor.ProfilePictureUrl,
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
                    }).ToList() ?? new List<SupportTypeDto>(),

                    // ✅ Organization member info for reporter
                    IsOrganizationMember = orgMember != null,
                    OrganizationName = orgMember?.Organization?.Name,
                    OrganizationLogoUrl = orgMember?.Organization?.LogoUrl
                };
            }).ToList();
        }
        public async Task<List<CategoryCountDto>> GetCategoryCountsAsync(int? year = null, int? month = null)
        {
            return await _postRepo.GetCategoryCountsAsync(year, month);
        }
        public async Task<List<(DateTime ReportDate, int ReportCount)>> GetReportCountLast7DaysAsync()
        {
            return await _postRepo.GetReportCountLast7DaysAsync();
        }
        public async Task<IEnumerable<DisasterReportDto>> GetRelatedReportsByTopicAsync(int reportId)
        {
            // Step 1: Get current report
            var currentReport = await _postRepo.DbContext.DisastersReports
                .FirstOrDefaultAsync(r => r.Id == reportId);

            if (currentReport == null || currentReport.DisasterTopicsId == null)
            {
                var allReports = await _postRepo.DbContext.DisastersReports
                    .OrderByDescending(r => r.ReportedAt)
                    .Take(10)
                    .Include(r => r.Location)
                    .Include(r => r.Reporter)
                    .Include(r => r.DisasterTopics)
                    .Include(r => r.ImpactUrls)
                    .Include(r => r.ImpactTypes)
                    .Include(r => r.SupportTypes)
                    .ToListAsync();

                return await MapToDtoListAsync(allReports);
            }

            var topicId = currentReport.DisasterTopicsId;

            // Step 2: Related reports
            var relatedReports = await _postRepo.DbContext.DisastersReports
                .Where(r => r.DisasterTopicsId == topicId && r.Id != reportId && r.Status==1)
                .Include(r => r.Location)
                .Include(r => r.Reporter)
                .Include(r => r.DisasterTopics)
                .Include(r => r.ImpactUrls)
                .Include(r => r.ImpactTypes)
                .Include(r => r.SupportTypes)
                .ToListAsync();

            var resultList = relatedReports;

            // Step 3: If not enough related reports, fill from fallback
            if (relatedReports.Count < 10)
            {
                int remaining = 10 - relatedReports.Count;

                var fallbackReports = await _postRepo.DbContext.DisastersReports
                    .Where(r => r.Id != reportId && r.DisasterTopicsId != topicId) // not same topic
                    .OrderByDescending(r => r.ReportedAt)
                    .Take(remaining)
                    .Include(r => r.Location)
                    .Include(r => r.Reporter)
                    .Include(r => r.DisasterTopics)
                    .Include(r => r.ImpactUrls)
                    .Include(r => r.ImpactTypes)
                    .Include(r => r.SupportTypes)
                    .ToListAsync();

                resultList.AddRange(fallbackReports);
            }

            // Step 4: Return only 10 reports
            return await MapToDtoListAsync(resultList.Take(10).ToList());
        }
        public async Task<IEnumerable<DisasterReportDto>> GetReportsByTopicIdAsync(int topicId)
        {
            var reports = await _postRepo.GetReportsByTopicIdAsync(topicId);
            return await MapToDtoListAsync(reports);
        }
        public async Task<List<DisasterReportMapDto>> GetDisasterReportsForMapAsync(ReportFilterDto filter)
        {
            return await _postRepo.GetFilteredDisasterReportsAsync(filter);
        }
        private void ClearAllReportsCache()
        {
            // MemoryCache does not support enumeration directly, so we need to use reflection to access the keys.
            if (_cache is MemoryCache memoryCache)
            {
                var cacheEntriesField = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var cacheEntries = cacheEntriesField?.GetValue(memoryCache) as dynamic;

                if (cacheEntries != null)
                {
                    foreach (var cacheItem in cacheEntries)
                    {
                        var cacheItemKey = cacheItem.GetType().GetProperty("Key")?.GetValue(cacheItem, null);
                        if (cacheItemKey != null && cacheItemKey.ToString().StartsWith("all_reports_"))
                        {
                            _cache.Remove(cacheItemKey);
                        }
                    }
                }
            }
        }

        private void ClearReportCache(int reportId, Guid reporterId)
        {
            _cache.Remove($"report_{reportId}");
            _cache.Remove($"reports_by_reporter_{reporterId}");
            // _cache.Remove("all_reports");
            ClearAllReportsCache();
            _cache.Remove("urgent_reports");
            _cache.Remove($"pending_reject_reports_by_reporter_{reporterId}");
        }

    }
}
