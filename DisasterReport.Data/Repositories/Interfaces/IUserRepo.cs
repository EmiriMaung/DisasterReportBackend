using DisasterReport.Data.Domain;

namespace DisasterReport.Data.Repositories.Interfaces
{
    public interface IUserRepo
    {
        Task<(List<User> Items, int TotalCount)> GetPaginatedUsersAsync(int page, int pageSize);

        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<(List<User> Items, int TotalCount)> GetPaginatedNormalUsersAsync(int page, int pageSize);

        Task<(List<User> Items, int TotalCount)> GetPaginatedActiveUsersAsync(int page, int pageSize);

        Task<IEnumerable<User>> GetAllActiveUsersAsync();

        Task<IEnumerable<User>> GetAllAdminsAsync();

        Task<(List<User> Items, int TotalCount)> GetPaginatedBlacklistedUsersAsync(int page, int pageSize);

        Task<IEnumerable<User>> GetAllBlacklistedUsers();

        Task<User?> GetUserByIdAsync(Guid id);

        Task<User?> GetUsersByEmailAsync(string email);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(Guid id);
    }
}
