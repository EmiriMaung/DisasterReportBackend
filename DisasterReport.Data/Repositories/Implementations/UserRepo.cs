using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DisasterReport.Data.Repositories.Implementations
{
    public class UserRepo : IUserRepo
    {
        private readonly ApplicationDBContext _context;
        public UserRepo(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<(List<User> Items, int TotalCount)> GetPaginatedUsersAsync(int page, int pageSize)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .AsNoTracking();

            int totalCount = await query.CountAsync();

            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, totalCount);
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<(List<User> Items, int TotalCount)> GetPaginatedNormalUsersAsync(int page, int pageSize)
        {
            var query = _context.Users
                .Where(u => u.RoleId == 2)
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .AsNoTracking();
            int totalCount = await query.CountAsync();
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (users, totalCount);
        }

        public async Task<(List<User> Items, int TotalCount)> GetPaginatedActiveUsersAsync(int page, int pageSize)
        {
            var query = _context.Users
                .Where(u => u.RoleId == 2)
                .Where(u => !_context.BlacklistEntries
                    .Any(be => be.UserId == u.Id && !be.IsDeleted))
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .AsNoTracking();

            int total = await query.CountAsync();
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => u.RoleId == 2)
                .Where(u => !_context.BlacklistEntries
                    .Any(be => be.UserId == u.Id && !be.IsDeleted))
                .Include(u => u.Role)  
                .Include(u => u.Organizations)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAdminsAsync()
        {
            return await _context.Users
                .Where(u => u.Role.RoleName == "Admin")
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .AsNoTracking()
                .ToListAsync();
        }


        public async Task<(List<User> Items, int TotalCount)> GetPaginatedBlacklistedUsersAsync(int page, int pageSize)
        {
            var query = _context.Users
                .Where(u => _context.BlacklistEntries.Any(be => be.UserId == u.Id && !be.IsDeleted))
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .AsNoTracking();

            int total = await query.CountAsync();

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }


        public async Task<IEnumerable<User>> GetAllBlacklistedUsers()
        {
            return await _context.Users
                .Where(u => _context.BlacklistEntries
                    .Any(be => be.UserId == u.Id && !be.IsDeleted))
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetUsersByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task UpdateUserAsync(User user)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw;
            }
        }

        //Actually, we don't delete users in our app. This is just for testing purposes.
        //We have to use gmail again and again cause of insufficient gamil account.
        public async Task DeleteUserAsync(Guid id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _context.Users
                    .Include(u => u.RefreshTokens)
                    .Include(u => u.ExternalLogins)
                    .Include(u => u.BlacklistEntries)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user != null)
                {
                    _context.RefreshTokens.RemoveRange(user.RefreshTokens);
                    _context.ExternalLogins.RemoveRange(user.ExternalLogins);
                    _context.BlacklistEntries.RemoveRange(user.BlacklistEntries);

                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                Console.WriteLine($"Error deleting user: {ex.Message}");
                throw;
            }
        }
    }
}
