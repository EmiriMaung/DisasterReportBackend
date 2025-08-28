using DisasterReport.Services.Models.Common;
using DisasterReport.Services.Models.UserDTO;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IUserService
    {
        //Task<PaginatedResult<UserDto>> GetPaginatedUsersAsync(int page, int pageSize);

        //Task<IEnumerable<UserDto>> GetAllUsersAsync();

        Task<PaginatedResult<UserDto>> GetPaginatedNormalUsersAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        );

        Task<PaginatedResult<UserDto>> GetPaginatedActiveUsersAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        );


        //Task<IEnumerable<UserDto>> GetAllActiveUsersAsync();

        Task<PaginatedResult<UserDto>> GetPaginatedAdminsAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        );


        Task<IEnumerable<AdminDto>> GetAdminsForDropdownAsync();


        Task<IEnumerable<UserDto>> GetAllAdminsAsync();

        Task<PaginatedResult<UserDto>> GetPaginatedBlacklistedUsersAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        );

        //Task<IEnumerable<UserDto>> GetAllBlacklistedUsersAsync();

        Task<UserDto?> GetUserByIdAsync(Guid id);

        Task<UserDto?> GetUsersByEmailAsync(string email);

        Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserFormDto dto);

        Task<UserDto?> UpdateCurrentUserAsync(Guid userId, UpdateUserFormDto dto);

        Task<UserDto?> UpdateCurrentUserNameAsync(Guid userId, UpdateUserNameDto dto);

        Task DeleteUserAsync(Guid id);
    }
}