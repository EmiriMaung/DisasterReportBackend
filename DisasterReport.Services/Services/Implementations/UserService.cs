using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Models.UserDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DisasterReport.Services.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IMemoryCache _cache;
        public UserService(IUserRepo userRepo, IMemoryCache cache)
        {
            _userRepo = userRepo;
            _cache = cache;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            string cacheKey = "AllUsers";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<UserDto> cachedUsers))
            {
                return cachedUsers;
            }

            var users = await _userRepo.GetAllUsersAsync();
            
            var userDto = users
                .Select(MapToDto)
                .ToList();

            _cache.Set(cacheKey, userDto, TimeSpan.FromMinutes(10));

            return userDto;
        }


        public async Task<IEnumerable<UserDto>> GetAllActiveUsersAsync()
        {
            string cacheKey = "AllActiveUsers";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<UserDto> cachedUsers))
            {
                return cachedUsers;
            }

            var users = await _userRepo.GetAllActiveUsersAsync();
            
            var userDto = users
                .Select(MapToDto)
                .ToList();

            _cache.Set(cacheKey, userDto, TimeSpan.FromMinutes(10));

            return userDto;
        }


        public async Task<IEnumerable<UserDto>> GetAllAdminsAsync()
        {
            string cacheKey = "AllAdmins";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<UserDto> cachedUsers))
            {
                return cachedUsers;
            }

            var users = await _userRepo.GetAllAdminsAsync();
            
            var userDto = users
                .Select(MapToDto)
                .ToList();

            _cache.Set(cacheKey, userDto, TimeSpan.FromMinutes(10));

            return userDto;
        }


        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            string cacheKey = $"User:{id}";

            if (_cache.TryGetValue(cacheKey, out UserDto cachedUser))
            {
                return cachedUser;
            }

            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var userDto = MapToDto(user);

            _cache.Set(cacheKey, userDto, TimeSpan.FromMinutes(10));

            return userDto;
        }


        public async Task<UserDto?> GetUsersByEmailAsync(string email)
        {
            string cacheKey = cacheKey = $"UserEmail:{email}";

            if (_cache.TryGetValue(cacheKey, out UserDto cachedUser))
            {
                return cachedUser;
            }

            var user = await _userRepo.GetUsersByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var userDto = MapToDto(user);

            _cache.Set(cacheKey, userDto, TimeSpan.FromMinutes(10));

            return userDto;
        }


        public async Task UpdateUserAsync(Guid id, UpdateUserDto userDto)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
                return;

            if (!string.IsNullOrWhiteSpace(userDto.Name))
                user.Name = userDto.Name;

            if (!string.IsNullOrWhiteSpace(userDto.ProfilePictureUrl))
                user.ProfilePictureUrl = userDto.ProfilePictureUrl;

            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateUserAsync(user);

            _cache.Remove($"User:{user.Id}");
        }


        public async Task DeleteUserAsync(Guid id)
        {
            await _userRepo.DeleteUserAsync(id);

            _cache.Remove($"User:{id}");
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
                UpdatedAt = user.UpdatedAt,
                OrganizationNames = user.Organizations?.Select(o => o.Name).ToList() ?? new List<string>()
            };
        }
    }
}
