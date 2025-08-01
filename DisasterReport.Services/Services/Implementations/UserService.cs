using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Models.Common;
using DisasterReport.Services.Models.UserDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace DisasterReport.Services.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IMemoryCache _cache;
        private readonly IBlacklistEntryRepo _blacklistEntryRepo;
        private readonly ICloudinaryService _cloudinaryService;
        public UserService(IUserRepo userRepo, IMemoryCache cache, IBlacklistEntryRepo blacklistEntryRepo, ICloudinaryService cloudinaryService)
        {
            _userRepo = userRepo;
            _cache = cache;
            _blacklistEntryRepo = blacklistEntryRepo;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<PaginatedResult<UserDto>> GetPaginatedUsersAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (users, totalCount) = await _userRepo.GetPaginatedUsersAsync(page, pageSize);

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(await MapToDtoAsync(user));
            }

            return new PaginatedResult<UserDto>
            {
                Items = userDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            string cacheKey = "AllUsers";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<UserDto> cachedUsers))
            {
                return cachedUsers;
            }

            var users = await _userRepo.GetAllUsersAsync();

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(await MapToDtoAsync(user)); // Check blacklist here
            }

            _cache.Set(cacheKey, userDtos, TimeSpan.FromMinutes(10));

            return userDtos;
        }

        public async Task<PaginatedResult<UserDto>> GetPaginatedNormalUsersAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            var (users, totalCount) = await _userRepo.GetPaginatedNormalUsersAsync(page, pageSize);

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(await MapToDtoAsync(user));
            }

            return new PaginatedResult<UserDto>
            {
                Items = userDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };
        }


        public async Task<PaginatedResult<UserDto>> GetPaginatedActiveUsersAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (users, total) = await _userRepo.GetPaginatedActiveUsersAsync(page, pageSize);

            var userDtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                RoleName = user.Role?.RoleName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                CreatedAt = user.CreatedAt,
                OrganizationNames = user.Organizations?.Select(o => o.Name).ToList() ?? new List<string>(),
                IsBlacklistedUser = false

            }).ToList();

            return new PaginatedResult<UserDto>
            {
                Items = userDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }


        public async Task<IEnumerable<UserDto>> GetAllActiveUsersAsync()
        {
            string cacheKey = "AllActiveUsers";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<UserDto> cachedUsers))
            {
                return cachedUsers;
            }

            var users = await _userRepo.GetAllActiveUsersAsync();
            
            var userDots = new List<UserDto>();
            foreach (var user in users)
            {
                userDots.Add(await MapToDtoAsync(user));
            }

            _cache.Set(cacheKey, userDots, TimeSpan.FromMinutes(10));

            return userDots;
        }


        public async Task<IEnumerable<UserDto>> GetAllAdminsAsync()
        {
            string cacheKey = "AllAdmins";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<UserDto> cachedUsers))
            {
                return cachedUsers;
            }

            var users = await _userRepo.GetAllAdminsAsync();

            var userDots = new List<UserDto>();
            foreach (var user in users)
            {
                userDots.Add(await MapToDtoAsync(user));
            }

            _cache.Set(cacheKey, userDots, TimeSpan.FromMinutes(10));

            return userDots;
        }


        public async Task<PaginatedResult<UserDto>> GetPaginatedBlacklistedUsersAsync(int page, int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (users, total) = await _userRepo.GetPaginatedBlacklistedUsersAsync(page, pageSize);

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(await MapToDtoAsync(user));
            }

            return new PaginatedResult<UserDto>
            {
                Items = userDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
        }


        public async Task<IEnumerable<UserDto>> GetAllBlacklistedUsersAsync()
        {
            string cacheKey = "AllBlacklistedUsers";

            if (_cache.TryGetValue(cacheKey, out IEnumerable<UserDto> cachedUsers))
            {
                return cachedUsers;
            }

            var users = await _userRepo.GetAllBlacklistedUsers();

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(await MapToDtoAsync(user));
            }

            _cache.Set(cacheKey, userDtos, TimeSpan.FromMinutes(10));

            return userDtos;
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

            var userDto = await MapToDtoAsync(user);

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

            var userDto = await MapToDtoAsync(user);

            _cache.Set(cacheKey, userDto, TimeSpan.FromMinutes(10));

            return userDto;
        }


        public async Task<UserDto?> UpdateUserAsync(Guid id, UpdateUserFormDto dto)
        {
            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null) return null;

            user.Name = dto.Name;
            user.UpdatedAt = DateTime.UtcNow;

            if (dto.ProfilePicture != null)
            {
                var uploadResult = await _cloudinaryService.UploadProfilePictureAsync(dto.ProfilePicture);
                user.ProfilePictureUrl = uploadResult.SecureUrl.ToString();
            }

            await _userRepo.UpdateUserAsync(user);

            var cacheKey = $"User:{user.Id}";
            _cache.Remove(cacheKey);

            var updatedDto = await MapToDtoAsync(user);
            _cache.Set(cacheKey, updatedDto, TimeSpan.FromMinutes(10));

            return updatedDto;
        }


        public async Task<UserDto?> UpdateCurrentUserAsync(Guid userId, UpdateUserFormDto dto)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null) return null;

            user.Name = dto.Name;
            user.UpdatedAt = DateTime.UtcNow;

            // If a new picture was uploaded, call your CloudinaryService
            if (dto.ProfilePicture != null)
            {
                var uploadResult = await _cloudinaryService.UploadProfilePictureAsync(dto.ProfilePicture);
                user.ProfilePictureUrl = uploadResult.SecureUrl.ToString();
            }

            await _userRepo.UpdateUserAsync(user);

            // Invalidate and update the cache
            var cacheKey = $"User:{user.Id}";
            _cache.Remove(cacheKey);

            var updatedDto = await MapToDtoAsync(user);
            _cache.Set(cacheKey, updatedDto, TimeSpan.FromMinutes(10));

            return updatedDto;
        }


        //Actually, we don't delete users in our app. This is just for testing purposes.
        //We have to use gmail again and again cause of insufficient gamil account.
        public async Task DeleteUserAsync(Guid id)
        {
            await _userRepo.DeleteUserAsync(id);

            _cache.Remove($"User:{id}");
        }


        private async Task<UserDto> MapToDtoAsync(User user)
        {
            bool isBlacklisted = await _blacklistEntryRepo.IsUserBlacklistedAsync(user.Id);

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                RoleName = user.Role?.RoleName,
                ProfilePictureUrl = user.ProfilePictureUrl,
                IsBlacklistedUser = isBlacklisted,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                OrganizationNames = user.Organizations?.Select(o => o.Name).ToList() ?? new List<string>()
            };
        }
    }
}
