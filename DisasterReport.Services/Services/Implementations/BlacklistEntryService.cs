using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Models.BlacklistEntryDTO;
using DisasterReport.Services.Models.Common;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;
using System.Threading;

namespace DisasterReport.Services.Services.Implementations
    {
        public class BlacklistEntryService : IBlacklistEntryService
        {
            private readonly IBlacklistEntryRepo _blacklistEntryRepo;
            private readonly IUserRepo _userRepo;
            private readonly IReportService _reportService;
            private readonly IEmailServices _emailService;

        public BlacklistEntryService(IBlacklistEntryRepo blacklistEntryRepo, IUserRepo userRepo, IMemoryCache cache, IEmailServices emailService, IReportService reportService)
        {
            _blacklistEntryRepo = blacklistEntryRepo;
            _userRepo = userRepo;
            _emailService = emailService;
            _reportService = reportService;
        }

        public async Task<PaginatedResult<BlacklistEntryDto>> GetAllBlacklistEntriesAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder,
            string? statusFilter,
            DateTime? startDate,
            DateTime? endDate,
            Guid? adminId
        )
        {

            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (entries, totalCount) = await _blacklistEntryRepo.GetAllAsync(
                page, pageSize, searchQuery, sortBy, sortOrder, statusFilter, startDate, endDate, adminId
            );

            var adminIds = entries.Select(e => e.CreatedAdminId)
                          .Concat(entries.Where(e => e.UpdatedAdminId.HasValue).Select(e => e.UpdatedAdminId!.Value))
                          .Distinct()
                          .ToList();

            var adminNames = await _userRepo.GetUserNamesByIdsAsync(adminIds);

            var entryDtos = entries.Select(entry => new BlacklistEntryDto
            {
                Id = entry.Id,
                UserId = entry.UserId,
                UserName = entry.User.Name,
                UserEmail = entry.User.Email,
                ProfilePictureUrl = entry.User.ProfilePictureUrl,
                Reason = entry.Reason,
                CreatedAt = entry.CreatedAt,
                IsDeletedFromBlacklist = entry.IsDeleted,
                CreatedAdminId = entry.CreatedAdminId,
                CreatedAdminName = adminNames.TryGetValue(entry.CreatedAdminId, out var name) ? name : "Unknown Admin",
                UpdatedAt = entry.UpdateAt, 
                UpdatedAdminId = entry.UpdatedAdminId,
                UpdatedAdminName = entry.UpdatedAdminId.HasValue && adminNames.TryGetValue(entry.UpdatedAdminId.Value, out var updatedName) ? updatedName : null
            }).ToList();


            var result = new PaginatedResult<BlacklistEntryDto>
            {
                Items = entryDtos,
                TotalItems = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return result;
        }


        public async Task<List<BlacklistExportDto>> GetAllBlacklistForExportAsync()
        {
            var entries = await _blacklistEntryRepo.GetAllForExportAsync();

            var adminIds = entries
                .Select(e => e.CreatedAdminId)
                .Concat(entries.Where(e => e.UpdatedAdminId.HasValue)
                               .Select(e => e.UpdatedAdminId.Value))
                .Distinct()
                .ToList();

            var adminInfoDict = await _userRepo.GetAdminInfoByIdsAsync(adminIds);

            return entries.Select(e => new BlacklistExportDto
            {
                Name = e.User.Name,
                Email = e.User.Email,
                ProfilePictureUrl = e.User.ProfilePictureUrl,
                Reason = e.Reason,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdateAt,
                IsDeletedFromBlacklist = e.IsDeleted,
                CreatedAdminName = adminInfoDict.TryGetValue(e.CreatedAdminId, out var createdAdmin) ? createdAdmin.Name : "Unknown",
                UpdatedAdminName = e.UpdatedAdminId.HasValue && adminInfoDict.TryGetValue(e.UpdatedAdminId.Value, out var updatedAdmin) ? updatedAdmin.Name : null
            }).ToList();
        }



        public async Task<BlacklistStatsDto> GetBlacklistStatsAsync()
        {
            var statsTuple = await _blacklistEntryRepo.GetBlacklistStatsAsync();

            var statsDto = new BlacklistStatsDto
            {
                TotalBlocked = statsTuple.TotalBlocked,
                TotalUnlocked = statsTuple.TotalUnlocked,
                BlockedLast7Days = statsTuple.BlockedLast7Days,
                UnblockedLast7Days = statsTuple.UnblockedLast7Days
            };

            return statsDto;
        }


        public async Task<BlacklistEntryDto> GetBlacklistEntryByIdAsync(int id)
        {
            var entry = await _blacklistEntryRepo.GetByIdAsync(id);
            if (entry == null)
            {
                throw new KeyNotFoundException($"Blacklist entry with ID {id} not found.");
            }

            var entryDto = MapToDto(entry);

            return entryDto;
        }


        public async Task<BlacklistDetailDto> GetBlacklistDetailByIdAsync(int id)
        {
            var entry = await _blacklistEntryRepo.GetByIdAsync(id);
            if (entry == null)
            {
                throw new KeyNotFoundException($"Blacklist entry with ID {id} not found.");
            }

            var adminIds = new List<Guid> { entry.CreatedAdminId };
            if (entry.UpdatedAdminId.HasValue)
            {
                adminIds.Add(entry.UpdatedAdminId.Value);
            }

            var adminInfoDict = await _userRepo.GetAdminInfoByIdsAsync(adminIds.Distinct());

            return new BlacklistDetailDto
            {
                Id = entry.Id,
                Name = entry.User.Name,
                Email = entry.User.Email,
                ProfilePictureUrl = entry.User.ProfilePictureUrl,
                Reason = entry.Reason,
                UpdatedReason = entry.UpdatedReason,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdateAt,
                IsDeletedFromBlacklist = entry.IsDeleted,

                CreatedAdminName = adminInfoDict.TryGetValue(entry.CreatedAdminId, out var createdAdmin)
                    ? createdAdmin.Name
                    : "Unknown",

                UpdatedAdminName = entry.UpdatedAdminId.HasValue && adminInfoDict.TryGetValue(entry.UpdatedAdminId.Value, out var updatedAdmin)
                    ? updatedAdmin.Name
                    : null
            };
        }


        public async Task AddAsync(CreateBlacklistEntryDto dto)
        {
            bool isAlreadyInBlacklist = await _blacklistEntryRepo.IsUserBlacklistedAsync(dto.UserId);
            if (isAlreadyInBlacklist)
            {
                throw new InvalidOperationException("This user is already blacklisted.");
            }

            var blacklistEntry = new BlacklistEntry
            {
                UserId = dto.UserId,
                Reason = dto.Reason,
                CreatedAdminId = dto.CreatedAdminId,
                CreatedAt = DateTime.UtcNow,
            };
            await _blacklistEntryRepo.AddAsync(blacklistEntry);

            string actionTaken = $"User was banned";
            await _reportService.ResolveReportAsync(dto.ReportId, dto.CreatedAdminId, actionTaken);

            try
            {
                var bannedUser = await _userRepo.GetUserByIdAsync(dto.UserId);
                if (bannedUser != null)
                {
                    var subject = "Account Notification: Your Access Has Been Banned";
                    var body = $@"
                        <p>Hello {bannedUser.Name},</p>
                        <p>This email is to inform you that your account on the platform has been restricted by an administrator.</p>
                        <p><b>Reason provided:</b> {dto.Reason}</p>
                        <p>If you believe this action was made in error, please contact our support team.</p>
                        <p>Sincerely,<br/>The Moderation Team</p>";

                    await _emailService.SendEmailAsync(bannedUser.Email, subject, body);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        public async Task UpdateReasonAsync(int id, UnblockUserDto dto)
            {
                var entry = await _blacklistEntryRepo.GetByIdAsync(id);
                if (entry == null || entry.IsDeleted)
                {
                    throw new KeyNotFoundException($"Blacklist entry with ID {id} not found or already removed from blacklist entry.");
                }

                entry.Reason = dto.UnblockedReason;
                entry.UpdateAt = DateTime.UtcNow;

                await _blacklistEntryRepo.UpdateAsync(entry);

                var updatedEntry = await _blacklistEntryRepo.GetByIdAsync(id);
                if (updatedEntry != null)
                {
                    var updatedDto = MapToDto(updatedEntry);
                }
            }


        public async Task SoftDeleteAsync(Guid userId, Guid adminId, string unblockedReason)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            await _blacklistEntryRepo.SoftDeleteByUserIdAsync(userId, adminId, unblockedReason);

        }


        public async Task<bool> IsUserBlacklistedAsync(Guid userId)
            {
                return await _blacklistEntryRepo.IsUserBlacklistedAsync(userId);
            }


            private static BlacklistEntryDto MapToDto(BlacklistEntry entry)
            {
                return new BlacklistEntryDto
                {
                    Id = entry.Id,
                    UserId = entry.UserId,
                    UserEmail = entry.User.Email,
                    UserName =entry.User.Name,
                    ProfilePictureUrl = entry.User.ProfilePictureUrl,
                    CreatedAdminId = entry.CreatedAdminId,
                    UpdatedAdminId = entry.UpdatedAdminId,
                    Reason = entry.Reason,
                    CreatedAt = entry.CreatedAt,
                    UpdatedAt = entry.UpdateAt,
                    IsDeletedFromBlacklist = entry.IsDeleted
                };
            }
        }
    }