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
            private readonly IMemoryCache _cache;
            private static CancellationTokenSource _cacheResetTokenSource = new CancellationTokenSource();

        public BlacklistEntryService(IBlacklistEntryRepo blacklistEntryRepo, IUserRepo userRepo, IMemoryCache cache)
            {
                _blacklistEntryRepo = blacklistEntryRepo;
                _userRepo = userRepo;
                _cache = cache;
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
            string cacheKey = $"BlacklistEntries_{page}_{pageSize}_{searchQuery}_{sortBy}_{sortOrder}_{statusFilter}_{startDate:yyyy-MM-dd}_{endDate:yyyy-MM-dd}_{adminId}";

            if (_cache.TryGetValue(cacheKey, out PaginatedResult<BlacklistEntryDto> cachedResult))
            {
                return cachedResult;
            }

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
                UnblockReason = entry.UpdatedReason,
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

            var cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
            .AddExpirationToken(new CancellationChangeToken(_cacheResetTokenSource.Token));

            _cache.Set(cacheKey, result, cacheEntryOptions);

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
                UpdatedReason = e.UpdatedReason,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdateAt,
                IsDeletedFromBlacklist = e.IsDeleted,
                CreatedAdminName = adminInfoDict.TryGetValue(e.CreatedAdminId, out var createdAdmin) ? createdAdmin.Name : "Unknown",
                UpdatedAdminName = e.UpdatedAdminId.HasValue && adminInfoDict.TryGetValue(e.UpdatedAdminId.Value, out var updatedAdmin) ? updatedAdmin.Name : null
            }).ToList();
        }



        public async Task<BlacklistStatsDto> GetBlacklistStatsAsync()
        {
            const string cacheKey = "BlacklistStats";
            if (_cache.TryGetValue(cacheKey, out BlacklistStatsDto cachedStats))
            {
                return cachedStats;
            }

            var statsTuple = await _blacklistEntryRepo.GetBlacklistStatsAsync();

            var statsDto = new BlacklistStatsDto
            {
                TotalBlocked = statsTuple.TotalBlocked,
                TotalUnlocked = statsTuple.TotalUnlocked,
                BlockedLast7Days = statsTuple.BlockedLast7Days,
                UnblockedLast7Days = statsTuple.UnblockedLast7Days
            };

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(10))
                .AddExpirationToken(new CancellationChangeToken(_cacheResetTokenSource.Token));

            _cache.Set(cacheKey, statsDto, cacheEntryOptions);

            return statsDto;
        }


        public async Task<BlacklistEntryDto> GetBlacklistEntryByIdAsync(int id)
            {
                string cacheKey = $"BlacklistEntry_{id}";

                if (_cache.TryGetValue(cacheKey, out BlacklistEntryDto cachedEntry))
                {
                    return cachedEntry;
                }

                var entry = await _blacklistEntryRepo.GetByIdAsync(id);
                if (entry == null)
                {
                    throw new KeyNotFoundException($"Blacklist entry with ID {id} not found.");
                }

                var entryDto = MapToDto(entry);

                _cache.Set(cacheKey, entryDto, TimeSpan.FromMinutes(10));

                return entryDto;
            }


        //public async Task<IEnumerable<BlacklistHistoryDto>> GetUserBlacklistHistoryAsync(Guid userId)
        //{
        //    var entries = await _blacklistEntryRepo.GetBlacklistEntriesByUserIdAsync(userId);
        //    if (!entries.Any())
        //    {
        //        return Enumerable.Empty<BlacklistHistoryDto>();
        //    }

        //    var adminIds = entries.Select(e => e.CreatedAdminId)
        //        .Concat(entries.Where(e => e.UpdatedAdminId.HasValue).Select(e => e.UpdatedAdminId!.Value))
        //        .Distinct().ToList();

        //    var admins = await _userRepo.GetUserNamesByIdsAsync(adminIds);

        //    var historyEvents = new List<BlacklistHistoryDto>();
        //    foreach (var entry in entries)
        //    {
        //        historyEvents.Add(new BlacklistHistoryDto
        //        {
        //            EventType = "Blacklisted",
        //            EventDate = entry.CreatedAt,
        //            UpdatedReason = entry.Reason,
        //            AdminName = admins.TryGetValue(entry.CreatedAdminId, out var createdAdminName) ? createdAdminName : "Unknown Admin"
        //        });

        //        if (entry.IsDeleted && entry.UpdateAt.HasValue && entry.UpdatedAdminId.HasValue)
        //        {
        //            historyEvents.Add(new BlacklistHistoryDto
        //            {
        //                EventType = "Unblocked",
        //                EventDate = entry.UpdateAt.Value,
        //                UpdatedReason = entry.UpdatedReason,
        //                AdminName = admins.TryGetValue(entry.UpdatedAdminId.Value, out var updatedAdminName) ? updatedAdminName : "Unknown Admin"
        //            });
        //        }
        //    }
        //    return historyEvents.OrderByDescending(e => e.EventDate);
        //}


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

            _cacheResetTokenSource.Cancel();
            _cacheResetTokenSource = new CancellationTokenSource();
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

                _cache.Remove($"BlacklistEntry_{id}");

                var updatedEntry = await _blacklistEntryRepo.GetByIdAsync(id);
                if (updatedEntry != null)
                {
                    var updatedDto = MapToDto(updatedEntry);
                    _cacheResetTokenSource.Cancel();
                    _cacheResetTokenSource = new CancellationTokenSource();
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

            _cacheResetTokenSource.Cancel();
            _cacheResetTokenSource = new CancellationTokenSource();
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