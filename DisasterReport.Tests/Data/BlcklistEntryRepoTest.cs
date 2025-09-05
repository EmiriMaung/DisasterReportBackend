using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DisasterReport.Tests.Data
{
    public class BlacklistEntryRepoTest
    {
        private ApplicationDBContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDBContext(options);
        }

        [Fact]
        public async Task AddAsync_ShouldAddEntry()
        {
            var context = GetDbContext();
            var repo = new BlacklistEntryRepo(context);

            var entry = new BlacklistEntry
            {
                UserId = Guid.NewGuid(),
                Reason = "Violation",
                CreatedAt = DateTime.UtcNow
            };

            await repo.AddAsync(entry);

            var result = await context.BlacklistEntries.FirstOrDefaultAsync();
            Assert.NotNull(result);
            Assert.Equal(entry.UserId, result.UserId);
        }


        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectEntry()
        {
            var context = GetDbContext();

            var userId = Guid.NewGuid();
            var user = new User
            {
                Id = userId,
                Name = "Test User",
                Email = "test@example.com",
                RoleId = 2,
                CreatedAt = DateTime.UtcNow
            };
            context.Users.Add(user);

            var entry = new BlacklistEntry
            {
                Id = 1,
                UserId = userId,
                CreatedAdminId = Guid.NewGuid(),
                Reason = "Test Reason",
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };
            context.BlacklistEntries.Add(entry);

            await context.SaveChangesAsync();

            var repo = new BlacklistEntryRepo(context);
            var result = await repo.GetByIdAsync(entry.Id);

            Assert.NotNull(result);
            Assert.Equal(entry.Id, result!.Id);
            Assert.Equal(userId, result.UserId);
        }


        [Fact]
        public async Task UpdateAsync_ShouldUpdateEntry()
        {
            var context = GetDbContext();
            var entry = new BlacklistEntry { UserId = Guid.NewGuid(), Reason = "Old Reason" };
            context.BlacklistEntries.Add(entry);
            await context.SaveChangesAsync();

            var repo = new BlacklistEntryRepo(context);
            entry.Reason = "New Reason";
            await repo.UpdateAsync(entry);

            var updated = await context.BlacklistEntries.FindAsync(entry.Id);
            Assert.Equal("New Reason", updated!.Reason);
        }

        [Fact]
        public async Task SoftDeleteByUserIdAsync_ShouldMarkEntryDeleted()
        {
            var context = GetDbContext();
            var userId = Guid.NewGuid();
            var adminId = Guid.NewGuid();

            var unblockedReason = "Test reason for unblocking.";

            var entry = new BlacklistEntry
            {
                UserId = userId,
                IsDeleted = false,
                CreatedAt = DateTime.UtcNow,
                Reason = "Test Reason"
            };
            context.BlacklistEntries.Add(entry);
            await context.SaveChangesAsync();

            var repo = new BlacklistEntryRepo(context);
            await repo.SoftDeleteByUserIdAsync(userId, adminId, unblockedReason);

            var deleted = await context.BlacklistEntries.FirstOrDefaultAsync(e => e.UserId == userId);
            Assert.True(deleted!.IsDeleted);
            Assert.Equal(adminId, deleted.UpdatedAdminId);
            Assert.NotNull(deleted.UpdateAt);
        }

        [Fact]
        public async Task IsUserBlacklistedAsync_ReturnsTrueIfNotDeleted()
        {
            var context = GetDbContext();
            var userId = Guid.NewGuid();

            context.BlacklistEntries.Add(new BlacklistEntry
            { 
                UserId = userId,
                IsDeleted = false,
                Reason = "Test",
            });
            await context.SaveChangesAsync();

            var repo = new BlacklistEntryRepo(context);
            var isBlacklisted = await repo.IsUserBlacklistedAsync(userId);

            Assert.True(isBlacklisted);
        }


        [Fact]
        public async Task IsUserBlacklistedAsync_ReturnsFalseIfDeleted()
        {
            var context = GetDbContext();
            var userId = Guid.NewGuid();

            context.BlacklistEntries.Add(new BlacklistEntry
            {
                UserId = userId,
                IsDeleted = true,
                Reason = "Test",
            });
            await context.SaveChangesAsync();

            var repo = new BlacklistEntryRepo(context);
            var isBlacklisted = await repo.IsUserBlacklistedAsync(userId);

            Assert.False(isBlacklisted);
        }
    }
}
