using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Models.Common;
using DisasterReport.Services.Models.UserDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Primitives;

namespace DisasterReport.Services.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly IBlacklistEntryRepo _blacklistEntryRepo;
        private readonly ICloudinaryService _cloudinaryService;

        public UserService(IUserRepo userRepo, IMemoryCache cache, IBlacklistEntryRepo blacklistEntryRepo, ICloudinaryService cloudinaryService)
        {
            _userRepo = userRepo;
            _blacklistEntryRepo = blacklistEntryRepo;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<PaginatedResult<UserDto>> GetPaginatedNormalUsersAsync(int page, int pageSize, string? searchQuery, string? sortBy, string? sortOrder)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;
            var (users, totalCount) = await _userRepo.GetPaginatedNormalUsersAsync(page, pageSize, searchQuery, sortBy, sortOrder);

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(await MapToDtoAsync(user));
            }

            var result =  new PaginatedResult<UserDto>
            {
                Items = userDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalCount
            };

            return result;
        }


        public async Task<PaginatedResult<UserDto>> GetPaginatedActiveUsersAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        )
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (users, total) = await _userRepo.GetPaginatedActiveUsersAsync(page, pageSize, searchQuery, sortBy, sortOrder);

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(await MapToDtoAsync(user));
            }

            var result = new PaginatedResult<UserDto>
            {
                Items = userDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };
            
            return result;
        }

        public async Task<PaginatedResult<UserDto>> GetPaginatedAdminsAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        )
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (users, total) = await _userRepo.GetPaginatedAdminsAsync(page, pageSize, searchQuery, sortBy, sortOrder);

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(await MapToDtoAsync(user));
            }

            var result = new PaginatedResult<UserDto>
            {
                Items = userDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };

            return result;
        }


        public async Task<IEnumerable<AdminDto>> GetAdminsForDropdownAsync()
        {
            var admins = await _userRepo.GetAllAdminsAsync();

            return admins.Select(u => new AdminDto
            {
                Id = u.Id,
                Name = u.Name
            });
        }

        public async Task<IEnumerable<UserDto>> GetAllAdminsAsync()
        {
            var users = await _userRepo.GetAllAdminsAsync();

            var userDots = new List<UserDto>();
            foreach (var user in users)
            {
                userDots.Add(await MapToDtoAsync(user));
            }

            return userDots;
        }

        public async Task<PaginatedResult<UserDto>> GetPaginatedBlacklistedUsersAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        )
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 10;

            var (users, total) = await _userRepo.GetPaginatedBlacklistedUsersAsync(page, pageSize, searchQuery, sortBy, sortOrder);

            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                userDtos.Add(await MapToDtoAsync(user));
            }

            var result = new PaginatedResult<UserDto>
            {
                Items = userDtos,
                Page = page,
                PageSize = pageSize,
                TotalItems = total
            };

            return result;
        }

        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            string cacheKey = $"User:{id}";

            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var userDto = await MapToDtoAsync(user);

            return userDto;
        }


        public async Task<UserDto?> GetUsersByEmailAsync(string email)
        {
            var user = await _userRepo.GetUsersByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var userDto = await MapToDtoAsync(user);

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

            var updatedDto = await MapToDtoAsync(user);
            
            return updatedDto;
        }

        public async Task<UserDto?> UpdateCurrentUserAsync(Guid userId, UpdateUserFormDto dto)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null) return null;

            user.Name = dto.Name;
            user.UpdatedAt = DateTime.UtcNow;

            if (dto.ProfilePicture != null)
            {
                var uploadResult = await _cloudinaryService.UploadProfilePictureAsync(dto.ProfilePicture);
                user.ProfilePictureUrl = uploadResult.SecureUrl.ToString();
            }

            await _userRepo.UpdateUserAsync(user);

            var updatedDto = await MapToDtoAsync(user);

            return updatedDto;
        }

        public async Task<UserDto?> UpdateCurrentUserNameAsync(Guid userId, UpdateUserNameDto dto)
        {
            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            user.Name = dto.Name;
            user.UpdatedAt = DateTime.UtcNow;

            await _userRepo.UpdateUserAsync(user);

            return await MapToDtoAsync(user);
        }

        public async Task DeleteUserAsync(Guid id)
        {
            await _userRepo.DeleteUserAsync(id);

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
                OrganizationNames = user.Organizations?
                    .Select(o => new OrganizationInfoDto
                    {
                        Id = o.Id,
                        Name = o.Name
                    })
                    .ToList() ?? new List<OrganizationInfoDto>()
            };
        }
    }
}
