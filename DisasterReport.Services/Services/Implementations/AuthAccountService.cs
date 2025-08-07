using DisasterReport.Data.Domain;
using DisasterReport.Services.Models.AuthDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DisasterReport.Services.Services.Implementations
{
    public class AuthAccountService : IAuthAccountService
    {
        private readonly ApplicationDBContext _context;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _config;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICloudinaryService _cloudinaryService;
        public AuthAccountService(ApplicationDBContext context, IJwtService jwtService, IConfiguration config, IPasswordHasherService passwordHasherService, IHttpClientFactory httpClientFactory, ICloudinaryService cloudinaryService)
        {
            _context = context;
            _jwtService = jwtService;
            _config = config;
            _passwordHasher = passwordHasherService;
            _httpClientFactory = httpClientFactory;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<TokenResultDto> LoginOrRegisterExternalAsync(OAuthUserInfoDto userInfo)
        {
            var login = _context.ExternalLogins
                .Include(x => x.User)
                .ThenInclude(u => u.Role)
                .FirstOrDefault(x => x.Provider == userInfo.Provider && x.ProviderKey == userInfo.ProviderKey);

            User user;

            if (login != null)
            {
                // Existing external login found
                user = login.User;

                if (await IsUserBlacklistedAsync(user.Id))
                    throw new Exception("This account is blacklisted.");
            }
            else
            {
                // Try to find user by email
                user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == userInfo.Email);

                if (user != null)
                {
                    if (await IsUserBlacklistedAsync(user.Id))
                        throw new Exception("This account is blacklisted.");
                }

                if (user == null)
                {
                    var permanentProfilePicUrl = await DownloadAndUploadProfilePictureAsync(userInfo.ProfilePictureUrl);

                    var adminEmails = _config["Admin:Email"]
                        ?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                        ?? Array.Empty<string>();

                    var isAdmin = adminEmails.Contains(userInfo.Email, StringComparer.OrdinalIgnoreCase);

                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = userInfo.Email,
                        Name = userInfo.Name,
                        RoleId = isAdmin ? 1 : 2,
                        ProfilePictureUrl = permanentProfilePicUrl,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    user.Role = await _context.UserRoles.FindAsync(user.RoleId);
                }

                var externalLogin = new ExternalLogin
                {
                    Provider = userInfo.Provider,
                    ProviderKey = userInfo.ProviderKey,
                    UserId = user.Id
                };

                _context.ExternalLogins.Add(externalLogin);
                await _context.SaveChangesAsync();
            }

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };
        }


        public async Task<TokenResultDto> RegisterAsync(RegisterDto registerDto)
        {
            var ExistingUser = _context.Users
                .FirstOrDefault(u => u.Email == registerDto.Email);

            if (ExistingUser != null)
                throw new Exception("Email is already Registered.");

            var defaultRole = _context.UserRoles.FirstOrDefault(r => r.Id == 2);
            if (defaultRole == null)
                throw new Exception("Default user role not found.");

            var passwordHash = _passwordHasher.HashPassword(registerDto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = registerDto.Email,
                Name = registerDto.Name,
                RoleId = defaultRole.Id,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };
        }


        public async Task<TokenResultDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
        .Include(u => u.Role)
        .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash))
                throw new Exception("Invalid email or password.");

            var isValid = _passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);
            if (!isValid)
                throw new Exception("Invalid email or password.");

            var isBlacklisted = await _context.BlacklistEntries
                .AnyAsync(be => be.UserId == user.Id && !be.IsDeleted);

            if (await IsUserBlacklistedAsync(user.Id))
                throw new Exception("This account is blacklisted.");

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshToken = _jwtService.GenerateRefreshToken();

            var refreshTokenEntity = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                CreatedAt = DateTime.UtcNow
            };

            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new TokenResultDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(10)
            };
        }

        private async Task<bool> IsUserBlacklistedAsync(Guid userId)
        {
            return await _context.BlacklistEntries
                .AnyAsync(be => be.UserId == userId && !be.IsDeleted);
        }


        private async Task<string?> DownloadAndUploadProfilePictureAsync(string? temporaryUrl)
        {
            if (string.IsNullOrEmpty(temporaryUrl))
            {
                return null;
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync(temporaryUrl);

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to download image from {temporaryUrl}. Status: {response.StatusCode}");
                    return null; 
                }

                await using var imageStream = await response.Content.ReadAsStreamAsync();
                var memoryStream = new MemoryStream();
                await imageStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var fileName = Path.GetFileName(new Uri(temporaryUrl).LocalPath);
                IFormFile profilePictureFile = new FormFile(memoryStream, 0, memoryStream.Length, "file", fileName);

                var uploadResult = await _cloudinaryService.UploadProfilePictureAsync(profilePictureFile);

                return uploadResult?.SecureUrl?.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing OAuth profile picture: {ex.Message}");
                return null;
            }
        }

    }
}
