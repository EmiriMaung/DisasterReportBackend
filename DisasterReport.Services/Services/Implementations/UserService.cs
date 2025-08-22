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
        //private readonly IMemoryCache _cache;
        private readonly IBlacklistEntryRepo _blacklistEntryRepo;
        private readonly ICloudinaryService _cloudinaryService;
        //private CancellationTokenSource _userListCacheTokenSource = new CancellationTokenSource();

        public UserService(IUserRepo userRepo, IMemoryCache cache, IBlacklistEntryRepo blacklistEntryRepo, ICloudinaryService cloudinaryService)
        {
            _userRepo = userRepo;
            //_cache = cache;
            _blacklistEntryRepo = blacklistEntryRepo;
            _cloudinaryService = cloudinaryService;
        }

        //public async Task<PaginatedResult<UserDto>> GetPaginatedUsersAsync(int page, int pageSize)
        //{
        //    if (page <= 0) page = 1;
        //    if (pageSize <= 0) pageSize = 10;

        //    var (users, totalCount) = await _userRepo.GetPaginatedUsersAsync(page, pageSize);

        //    var userDtos = new List<UserDto>();
        //    foreach (var user in users)
        //    {
        //        userDtos.Add(await MapToDtoAsync(user));
        //    }

        //    return new PaginatedResult<UserDto>
        //    {
        //        Items = userDtos,
        //        Page = page,
        //        PageSize = pageSize,
        //        TotalItems = totalCount
        //    };
        //}

        //public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        //{
        //    string cacheKey = "AllUsers";

        //    if (_cache.TryGetValue(cacheKey, out IEnumerable<UserDto> cachedUsers))
        //    {
        //        return cachedUsers;
        //    }

        //    var users = await _userRepo.GetAllUsersAsync();

        //    var userDtos = new List<UserDto>();
        //    foreach (var user in users)
        //    {
        //        userDtos.Add(await MapToDtoAsync(user));
        //    }

        //    _cache.Set(cacheKey, userDtos, TimeSpan.FromMinutes(10));

        //    return userDtos;
        //}

        public async Task<PaginatedResult<UserDto>> GetPaginatedNormalUsersAsync(int page, int pageSize, string? searchQuery, string? sortBy, string? sortOrder)
        {
            //var cacheKey = $"NormalUsers-Page{page}-PageSize{pageSize}-Search:{searchQuery}-SortBy:{sortBy}-SortOrder:{sortOrder}";

            //if (_cache.TryGetValue(cacheKey, out PaginatedResult<UserDto> cachedResult))
            //{
            //    return cachedResult;
            //}

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

            //var cacheOptions = new MemoryCacheEntryOptions()
            //.SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            //.AddExpirationToken(new CancellationChangeToken(_userListCacheTokenSource.Token));

            //_cache.Set(cacheKey, result, cacheOptions);

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
            //var cacheKey = $"ActiveUsers-Page{page}-PageSize{pageSize}-Search:{searchQuery}-SortBy:{sortBy}-SortOrder:{sortOrder}";

            //if (_cache.TryGetValue(cacheKey, out PaginatedResult<UserDto> cachedResult))
            //{
            //    return cachedResult;
            //}

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
            
            //var cacheOptions = new MemoryCacheEntryOptions()
            //.SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            //.AddExpirationToken(new CancellationChangeToken(_userListCacheTokenSource.Token));

            //_cache.Set(cacheKey, result, cacheOptions);

            return result;
        }


        //public async Task<IEnumerable<UserDto>> GetAllActiveUsersAsync()
        //{
        //    string cacheKey = "AllActiveUsers";

        //    if (_cache.TryGetValue(cacheKey, out IEnumerable<UserDto> cachedUsers))
        //    {
        //        return cachedUsers;
        //    }

        //    var users = await _userRepo.GetAllActiveUsersAsync();
            
        //    var userDots = new List<UserDto>();
        //    foreach (var user in users)
        //    {
        //        userDots.Add(await MapToDtoAsync(user));
        //    }

        //    _cache.Set(cacheKey, userDots, TimeSpan.FromMinutes(10));

        //    return userDots;
        //}


        public async Task<PaginatedResult<UserDto>> GetPaginatedAdminsAsync(
            int page,
            int pageSize,
            string? searchQuery,
            string? sortBy,
            string? sortOrder
        )
        {
            //var cacheKey = $"Admins-Page{page}-PageSize{pageSize}-Search:{searchQuery}-SortBy:{sortBy}-SortOrder:{sortOrder}";

            //if (_cache.TryGetValue(cacheKey, out PaginatedResult<UserDto> cachedResult))
            //{
            //    return cachedResult;
            //}

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

            //var cacheOptions = new MemoryCacheEntryOptions()
            //.SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            //.AddExpirationToken(new CancellationChangeToken(_userListCacheTokenSource.Token));

            //_cache.Set(cacheKey, result, cacheOptions);

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
            //string cacheKey = "AllAdmins";

            //if (_cache.TryGetValue(cacheKey, out IEnumerable<UserDto> cachedUsers))
            //{
            //    return cachedUsers;
            //}

            var users = await _userRepo.GetAllAdminsAsync();

            var userDots = new List<UserDto>();
            foreach (var user in users)
            {
                userDots.Add(await MapToDtoAsync(user));
            }

            //_cache.Set(cacheKey, userDots, TimeSpan.FromMinutes(10));

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
            //var cacheKey = $"BlacklistedUsers-Page{page}-PageSize{pageSize}-Search:{searchQuery}-SortBy:{sortBy}-SortOrder:{sortOrder}";

            //if (_cache.TryGetValue(cacheKey, out PaginatedResult<UserDto> cachedResult))
            //{
            //    return cachedResult;
            //}

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

            //var cacheOptions = new MemoryCacheEntryOptions()
            //.SetAbsoluteExpiration(TimeSpan.FromMinutes(5))
            //.AddExpirationToken(new CancellationChangeToken(_userListCacheTokenSource.Token));

            //_cache.Set(cacheKey, result, cacheOptions);

            return result;
        }


        //public async Task<IEnumerable<UserDto>> GetAllBlacklistedUsersAsync()
        //{
        //    string cacheKey = "AllBlacklistedUsers";

        //    if (_cache.TryGetValue(cacheKey, out IEnumerable<UserDto> cachedUsers))
        //    {
        //        return cachedUsers;
        //    }

        //    var users = await _userRepo.GetAllBlacklistedUsers();

        //    var userDtos = new List<UserDto>();
        //    foreach (var user in users)
        //    {
        //        userDtos.Add(await MapToDtoAsync(user));
        //    }

        //    _cache.Set(cacheKey, userDtos, TimeSpan.FromMinutes(10));

        //    return userDtos;
        //}


        public async Task<UserDto?> GetUserByIdAsync(Guid id)
        {
            string cacheKey = $"User:{id}";

            //if (_cache.TryGetValue(cacheKey, out UserDto cachedUser))
            //{
            //    return cachedUser;
            //}

            var user = await _userRepo.GetUserByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            var userDto = await MapToDtoAsync(user);

            //_cache.Set(cacheKey, userDto, TimeSpan.FromMinutes(10));

            return userDto;
        }


        public async Task<UserDto?> GetUsersByEmailAsync(string email)
        {
            //string cacheKey = $"UserEmail:{email}";

            //if (_cache.TryGetValue(cacheKey, out UserDto cachedUser))
            //{
            //    return cachedUser;
            //}

            var user = await _userRepo.GetUsersByEmailAsync(email);
            if (user == null)
            {
                return null;
            }

            var userDto = await MapToDtoAsync(user);

            //_cache.Set(cacheKey, userDto, TimeSpan.FromMinutes(10));

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

            //var userCacheKey = $"User:{user.Id}";
            //_cache.Remove(userCacheKey);
            var updatedDto = await MapToDtoAsync(user);
            //_cache.Set(userCacheKey, updatedDto, TimeSpan.FromMinutes(10));

            //InvalidateUserListCache();

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
            //var cacheKey = $"User:{user.Id}";
            //_cache.Remove(cacheKey);
            var updatedDto = await MapToDtoAsync(user);
            //_cache.Set(cacheKey, updatedDto, TimeSpan.FromMinutes(10));

            //InvalidateUserListCache();

            return updatedDto;
        }


        //Actually, we don't delete users in our app. This is just for testing purposes.
        //We have to use gmail again and again cause of insufficient gamil account.
        public async Task DeleteUserAsync(Guid id)
        {
            await _userRepo.DeleteUserAsync(id);

            //_cache.Remove($"User:{id}");
            //InvalidateUserListCache();
        }


        //private void InvalidateUserListCache()
        //{
        //    // Cancel the token, which expires all cache entries that are listening to it.
        //    _userListCacheTokenSource.Cancel();

        //    // Create a new token source for the next set of cache entries.
        //    _userListCacheTokenSource = new CancellationTokenSource();
        //}


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
