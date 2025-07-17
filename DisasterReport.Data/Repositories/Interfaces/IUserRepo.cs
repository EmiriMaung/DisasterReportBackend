using DisasterReport.Data.Domain;
using DisasterReport.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Repositories.Interfaces
{
    public interface IUserRepo
    {
        Task<IEnumerable<User>> GetAllUsersAsync(UserFilterOptions options);

        Task<User?> GetUserByIdAsync(Guid id);

        Task<User?> GetUsersByEmailAsync(string email);

        Task<User?> GetBlacklistedUserByIdAsync(Guid id);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(Guid id);
    }
}
