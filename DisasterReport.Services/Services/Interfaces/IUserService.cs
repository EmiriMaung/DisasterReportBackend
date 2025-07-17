using DisasterReport.Data.Models;
using DisasterReport.Services.Models.UserDTO;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync(UserFilterOptions options);

        Task<UserDto?> GetUserByIdAsync(Guid id);

        Task<UserDto?> GetUsersByEmailAsync(string email);

        Task<UserDto?> GetBlacklistedUserByIdAsync(Guid id);

        Task UpdateUserAsync(UpdateUserDto userDto);

        Task DeleteUserAsync(Guid id);
    }
}