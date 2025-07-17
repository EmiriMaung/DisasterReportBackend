using DisasterReport.Data.Domain;
using DisasterReport.Data.Models;
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

        public async Task<IEnumerable<User>> GetAllUsersAsync(UserFilterOptions options)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .AsQueryable();

            if (options.OnlyBlacklisted.HasValue)
            {
                query = query.Where(u => u.IsBlacklistedUser == options.OnlyBlacklisted.Value);
            }

            if (options.RoleId.HasValue)
            {
                query = query.Where(u => u.RoleId == options.RoleId.Value);
            }

            return await query.ToListAsync();
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

        public async Task<User?> GetBlacklistedUserByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsBlacklistedUser);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}
