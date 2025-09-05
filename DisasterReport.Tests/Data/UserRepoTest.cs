using CloudinaryDotNet.Actions;
using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Implementations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Threading.Tasks;
using Xunit;

namespace DisasterReport.Tests.Data
{
    public class UserRepoTests
    {
        private ApplicationDBContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            return new ApplicationDBContext(options);
        }


        [Fact]
        public async Task GetPaginatedNormalUsersAsync_ShouldReturnOnlyRoleId2Users()
        {
            var context = GetDbContext();

            var roleNormal = new UserRole { Id = 2, RoleName = "User" };
            var roleAdmin = new UserRole { Id = 1, RoleName = "Admin" };

            context.UserRoles.AddRange(roleNormal, roleAdmin);

            var normalUser = new User
            {
                Id = Guid.NewGuid(),
                Name = "Alice",
                Email = "alice@test.com",
                RoleId = 2,
                Role = roleNormal,  
                CreatedAt = DateTime.UtcNow
            };

            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Name = "Bob",
                Email = "bob@test.com",
                RoleId = 1,
                Role = roleAdmin,      
                CreatedAt = DateTime.UtcNow
            };

            context.Users.AddRange(normalUser, adminUser);
            await context.SaveChangesAsync();

            var repo = new UserRepo(context);

            var (items, totalCount) = await repo.GetPaginatedNormalUsersAsync(1, 10, null, null, null);

            Assert.Single(items);
            Assert.Equal("Alice", items[0].Name);
            Assert.Equal(1, totalCount);
        }


        [Fact]
        public async Task GetUserByIdAsync_ShouldReturnCorrectUser()
        {
            var context = GetDbContext();

            var role = new UserRole { Id = 2, RoleName = "User" };
            context.UserRoles.Add(role);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Charlie",
                Email = "charlie@test.com",
                RoleId = role.Id,
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repo = new UserRepo(context);

            var result = await repo.GetUserByIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal(user.Id, result!.Id);
            Assert.Equal("Charlie", result.Name);
        }


        [Fact]
        public async Task GetUsersByEmailAsync_ShouldReturnUser_WhenEmailExists()
        {
            // Arrange
            var context = GetDbContext();

            var role = new UserRole { Id = 2, RoleName = "User" };
            context.UserRoles.Add(role);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = "Test User",
                Email = "test@example.com",
                RoleId = role.Id,
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repo = new UserRepo(context);

            var result = await repo.GetUsersByEmailAsync("test@example.com");

            Assert.NotNull(result);
            Assert.Equal(user.Email, result!.Email);
            Assert.Equal("Test User", result.Name);
        }



        [Fact]
        public async Task UpdateUserAsync_ShouldUpdateUserSuccessfully()
        {
            var context = GetDbContext();
            var user = new User { Id = Guid.NewGuid(), Name = "Emma", Email = "emma@test.com", RoleId = 2, CreatedAt = DateTime.UtcNow };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repo = new UserRepo(context);
            user.Name = "Emma Updated";
            await repo.UpdateUserAsync(user);

            var updatedUser = await context.Users.FindAsync(user.Id);
            Assert.Equal("Emma Updated", updatedUser!.Name);
        }

        [Fact]
        public async Task DeleteUserAsync_ShouldRemoveUserAndDependencies()
        {
            var context = GetDbContext();
            var user = new User { Id = Guid.NewGuid(), Name = "Frank", Email = "frank@test.com", RoleId = 2, CreatedAt = DateTime.UtcNow };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var repo = new UserRepo(context);
            await repo.DeleteUserAsync(user.Id);

            var deletedUser = await context.Users.FindAsync(user.Id);
            Assert.Null(deletedUser);
        }
    }
}