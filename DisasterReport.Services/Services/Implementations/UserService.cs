using DisasterReport.Data.Domain;
using DisasterReport.Data.Models;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Models.UserDTO;
using DisasterReport.Services.Services.Interfaces;

namespace DisasterReport.Services.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        public UserService(IUserRepo userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync(UserFilterOptions options)
        {
            var users = await _userRepo.GetAllUsersAsync(options);
            return users
                .Select(MapToDto)
                .ToList();
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetUsersByEmailAsync(string email)
        {
            var user = await _userRepo.GetUsersByEmailAsync(email);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserDto?> GetBlacklistedUserByIdAsync(Guid id)
        {
            var user = await _userRepo.GetBlacklistedUserByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }
              
        public async Task UpdateUserAsync(UpdateUserDto userDto)
        {
            var user = await _userRepo.GetUserByIdAsync(userDto.Id);
            if (user == null)
                return;

            user.Name = userDto.Name ?? user.Name;
            user.ProfilePictureUrl = userDto.ProfilePictureUrl ?? user.ProfilePictureUrl;

            await _userRepo.UpdateUserAsync(user);
            return;
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await _userRepo.DeleteUserAsync(id);
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                RoleName = user.Role?.RoleName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsBlacklistedUser = user.IsBlacklistedUser,
                CreatedAt = user.CreatedAt,
                OrganizationNames = user.Organizations?.Select(o => o.Name).ToList() ?? new List<string>()
            };
        }
    }
}
