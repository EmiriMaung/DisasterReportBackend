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

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Include(u => u.Role)
                .Include(u => u.Organizations)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllActiveUsersAsync()
        {
            return await _context.Users
                .Where(u => !u.IsBlacklistedUser)
                .Where(u => u.RoleId == 2)
                .Include(u => u.Role)  
                .Include(u => u.Organizations)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAdminsAsync()
        {
            return await _context.Users
                .Where(u => u.Role.RoleName == "Admin")
                .Include(u => u.Role)
                .Include(u => u.Organizations)
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
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.RefreshTokens)
                .Include(u => u.ExternalLogins)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user != null)
            {
                _context.RefreshTokens.RemoveRange(user.RefreshTokens);
                _context.ExternalLogins.RemoveRange(user.ExternalLogins);

                _context.Users.Remove(user);

                await _context.SaveChangesAsync();
            }
        }

    }
}
