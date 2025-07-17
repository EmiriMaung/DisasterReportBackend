using DisasterReport.Data.Models;
using DisasterReport.Services.Models.UserDTO;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<IEnumerable<UserDto>> GetAllActiveUsersAsync();

        Task<IEnumerable<UserDto>> GetAllAdminsAsync();

        Task<UserDto?> GetUserByIdAsync(Guid id);

        Task<UserDto?> GetUsersByEmailAsync(string email);

        Task UpdateUserAsync(UpdateUserDto userDto);

        Task DeleteUserAsync(Guid id);
    }
}