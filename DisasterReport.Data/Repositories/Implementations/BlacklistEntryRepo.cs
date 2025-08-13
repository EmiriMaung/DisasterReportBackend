using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DisasterReport.Data.Repositories.Implementations
{
    public class BlacklistEntryRepo : IBlacklistEntryRepo
    {
        private readonly ApplicationDBContext _context;
        public BlacklistEntryRepo(ApplicationDBContext context)
        {
            _context = context;
        }


        public async Task<(List<BlacklistEntry> Items, int TotalCount)> GetAllAsync(
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
            var query = _context.BlacklistEntries
                .Include(be => be.User)
                .AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                var searchTermPattern = $"%{searchQuery.Trim()}%";
                query = query.Where(be =>
                    EF.Functions.Like(be.User.Name, searchTermPattern) ||
                    EF.Functions.Like(be.User.Email, searchTermPattern) ||
                    EF.Functions.Like(be.Reason, searchTermPattern)
                );
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.Equals("blocked", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(be => !be.IsDeleted);
                }
                else if (statusFilter.Equals("unblocked", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(be => be.IsDeleted);
                }
            }

            if (startDate.HasValue)
            {
                query = query.Where(be => be.CreatedAt >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                query = query.Where(be => be.CreatedAt < endDate.Value.AddDays(1));
            }

            if (!string.IsNullOrEmpty(sortBy))
            {
                switch (sortBy.ToLowerInvariant())
                {
                    case "username":
                        query = sortOrder?.ToLowerInvariant() == "desc"
                            ? query.OrderByDescending(be => be.User.Name)
                            : query.OrderBy(be => be.User.Name);
                        break;

                    case "email":
                        query = sortOrder?.ToLowerInvariant() == "desc"
                            ? query.OrderByDescending(be => be.User.Email)
                            : query.OrderBy(be => be.User.Email);
                        break;

                    case "date":
                    default:
                        query = sortOrder?.ToLowerInvariant() == "asc"
                            ? query.OrderBy(be => be.CreatedAt)
                            : query.OrderByDescending(be => be.CreatedAt);
                        break;
                }
            }
            else
            {
                query = query.OrderByDescending(be => be.CreatedAt);
            }

            if (adminId.HasValue)
            {
                query = query.Where(be => be.CreatedAdminId == adminId.Value);
            }

            var totalCount = await query.CountAsync();

            var entries = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (entries, totalCount);
        }


        public async Task<List<BlacklistEntry>> GetAllForExportAsync()
        {
            return await _context.BlacklistEntries
                .Include(be => be.User)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<(int TotalBlocked, int TotalUnlocked, int BlockedLast7Days, int UnblockedLast7Days)> GetBlacklistStatsAsync()
        {
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            int totalBlocked = await _context.BlacklistEntries.CountAsync(be => !be.IsDeleted);
            int totalUnlocked = await _context.BlacklistEntries.CountAsync(be => be.IsDeleted);
            int blockedLast7Days = await _context.BlacklistEntries.CountAsync(be => be.CreatedAt >= sevenDaysAgo);
            int unblockedLast7Days = await _context.BlacklistEntries.CountAsync(be =>
                be.IsDeleted && be.UpdateAt.HasValue && be.UpdateAt.Value >= sevenDaysAgo
            );

            return (totalBlocked, totalUnlocked, blockedLast7Days, unblockedLast7Days);
        }


        public async Task<BlacklistEntry?> GetByIdAsync(int id)
        {
            return await _context.BlacklistEntries
                .Include(be => be.User)
                .FirstOrDefaultAsync(be => be.Id == id);
        }

        public async Task<Dictionary<Guid, string>> GetUserNamesByIdsAsync(IEnumerable<Guid> userIds)
        {
            if (userIds == null || !userIds.Any())
            {
                return new Dictionary<Guid, string>();
            }

            return await _context.Users
                .Where(u => userIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Name);
        }

        public async Task<IEnumerable<BlacklistEntry>> GetBlacklistEntriesByUserIdAsync(Guid userId)
        {
            return await _context.BlacklistEntries
                .Where(be => be.UserId == userId)
                .ToListAsync();
        }

        public async Task AddAsync(BlacklistEntry entry)
        {
            await _context.BlacklistEntries.AddAsync(entry);
            await _context.SaveChangesAsync();
        }


        public async Task UpdateAsync(BlacklistEntry entry)
        {
            _context.BlacklistEntries.Update(entry);
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteByUserIdAsync(Guid userId, Guid adminId, string unblockedReason)
        {
            var entry = await _context.BlacklistEntries
                .FirstOrDefaultAsync(be => be.UserId == userId && !be.IsDeleted);

            if (entry != null)
            {
                entry.IsDeleted = true;
                entry.UpdatedAdminId = adminId;
                entry.UpdateAt = DateTime.UtcNow;
                entry.UpdatedReason = unblockedReason;

                _context.BlacklistEntries.Update(entry);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<bool> IsUserBlacklistedAsync(Guid userId)
        {
            return await _context.BlacklistEntries
                                .AnyAsync(be => be.UserId == userId && !be.IsDeleted);
        }
    }
}