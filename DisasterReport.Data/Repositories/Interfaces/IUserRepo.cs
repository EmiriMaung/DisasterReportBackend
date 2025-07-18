using DisasterReport.Data.Domain;

namespace DisasterReport.Data.Repositories.Interfaces
{
    public interface IUserRepo
    {
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<IEnumerable<User>> GetAllActiveUsersAsync();

        Task<IEnumerable<User>> GetAllAdminsAsync();

        Task<User?> GetUserByIdAsync(Guid id);

        Task<User?> GetUsersByEmailAsync(string email);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(Guid id);
    }
}
