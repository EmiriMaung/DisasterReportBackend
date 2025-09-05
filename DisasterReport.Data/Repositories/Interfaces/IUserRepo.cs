using DisasterReport.Data.Domain;

namespace DisasterReport.Data.Repositories.Interfaces
{
    public class AdminInfo
    {
        public string Name { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }

    public interface IUserRepo
    {
        Task<(List<User> Items, int TotalCount)> GetPaginatedNormalUsersAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        );  

        Task<(List<User> Items, int TotalCount)> GetPaginatedActiveUsersAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        );

        Task<(List<User> Items, int TotalCount)> GetPaginatedAdminsAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        );

        Task<IEnumerable<User>> GetAllAdminsAsync();

        Task<(List<User> Items, int TotalCount)> GetPaginatedBlacklistedUsersAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        );

        Task<Dictionary<Guid, string>> GetUserNamesByIdsAsync(List<Guid> userIds);

        Task<Dictionary<Guid, AdminInfo>> GetAdminInfoByIdsAsync(IEnumerable<Guid> userIds);

        Task<User?> GetUserByIdAsync(Guid id);

        Task<User?> GetUsersByEmailAsync(string email);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(Guid id);
    }
}
