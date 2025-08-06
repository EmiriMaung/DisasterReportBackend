    using DisasterReport.Data.Domain;
    using DisasterReport.Data.Repositories.Interfaces;
    using DisasterReport.Services.Models.BlacklistEntryDTO;
    using DisasterReport.Services.Services.Interfaces;
    using Microsoft.Extensions.Caching.Memory;

    namespace DisasterReport.Services.Services.Implementations
    {
        public class BlacklistEntryService : IBlacklistEntryService
        {
            private readonly IBlacklistEntryRepo _blacklistEntryRepo;
            private readonly IUserRepo _userRepo;
            private readonly IMemoryCache _cache;
            public BlacklistEntryService(IBlacklistEntryRepo blacklistEntryRepo, IUserRepo userRepo, IMemoryCache cache)
            {
                _blacklistEntryRepo = blacklistEntryRepo;
                _userRepo = userRepo;
                _cache = cache;
            }

            public async Task<IEnumerable<BlacklistEntryDto>> GetAllBlacklistEntriesAsync()
            {
                string cacheKey = "AllBlacklistEntries";

                if (_cache.TryGetValue(cacheKey, out IEnumerable<BlacklistEntryDto> cachedEntries))
                {
                    return cachedEntries;
                }

                var entries = await _blacklistEntryRepo.GetAllAsync();
                var entryDtos = entries
                    .Select(MapToDto)
                    .ToList();

                _cache.Set(cacheKey, entries, TimeSpan.FromMinutes(10));

                return entryDtos;
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
                    CreatedAt = DateTime.Now,
                };
                await _blacklistEntryRepo.AddAsync(blacklistEntry);
            }


            public async Task UpdateReasonAsync(int id, UpdateBlacklistEntryDto dto)
            {
                var entry = await _blacklistEntryRepo.GetByIdAsync(id);
                if (entry == null || entry.IsDeleted)
                {
                    throw new KeyNotFoundException($"Blacklist entry with ID {id} not found or already removed from blacklist entry.");
                }

                entry.Reason = dto.Reason;
                entry.UpdatedAdminId = dto.UpdatedAdminId;
                entry.UpdateAt = DateTime.UtcNow;

                await _blacklistEntryRepo.UpdateAsync(entry);

                _cache.Remove($"BlacklistEntry_{id}");

                var updatedEntry = await _blacklistEntryRepo.GetByIdAsync(id);
                if (updatedEntry != null)
                {
                    var updatedDto = MapToDto(updatedEntry);
                    _cache.Set($"BlacklistEntry_{id}", updatedDto, TimeSpan.FromMinutes(10));
                }
            }


        public async Task SoftDeleteAsync(Guid userId, Guid adminId)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }
            await _blacklistEntryRepo.SoftDeleteByUserIdAsync(userId, adminId);

            _cache.Remove($"User:{userId}");
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