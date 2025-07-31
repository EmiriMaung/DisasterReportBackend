using DisasterReport.Services.Models.UserDTO;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<IEnumerable<UserDto>> GetAllActiveUsersAsync();

        Task<IEnumerable<UserDto>> GetAllAdminsAsync();

        Task<IEnumerable<UserDto>> GetAllBlacklistedUsersAsync();

        Task<UserDto?> GetUserByIdAsync(Guid id);

        Task<UserDto?> GetUsersByEmailAsync(string email);

        Task UpdateUserAsync(Guid id, UpdateUserDto userDto);

        Task<UserDto?> UpdateCurrentUserAsync(Guid userId, UpdateUserFormDto dto);

        Task DeleteUserAsync(Guid id);
    }
}