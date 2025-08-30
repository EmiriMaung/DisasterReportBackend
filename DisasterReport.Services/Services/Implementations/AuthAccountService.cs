using DisasterReport.Data.Domain;
using DisasterReport.Services.Models.AuthDTO;
using DisasterReport.Services.Services.Interfaces;
using DisasterReport.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

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
        private readonly IEmailServices _emailServices;
        public AuthAccountService(ApplicationDBContext context, IJwtService jwtService, IConfiguration config, IPasswordHasherService passwordHasherService, IHttpClientFactory httpClientFactory, ICloudinaryService cloudinaryService, IEmailServices emailServices)
        {
            _context = context;
            _jwtService = jwtService;
            _config = config;
            _passwordHasher = passwordHasherService;
            _httpClientFactory = httpClientFactory;
            _cloudinaryService = cloudinaryService;
            _emailServices = emailServices;
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
                    throw new ForbiddenException("Your account has been blacklisted.");
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
                        throw new ForbiddenException("Your account has been blacklisted.");
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
        public async Task RequestOtpAsync(string email)
        {
            var otpCode = RandomNumberGenerator.GetInt32(100000, 999999).ToString("D6");
            var otpToken = new OtpToken
            {
                Email = email,
                Code = otpCode,
                ExpiresAt = DateTime.UtcNow.AddMinutes(1)
            };

            _context.OtpTokens.Add(otpToken);
            await _context.SaveChangesAsync();

            await _emailServices.SendOtpEmailAsync(email, otpCode);
        }


        public async Task<TokenResultDto> AuthenticateWithOtpAsync(string email, string code)
        {
            var otpToken = await _context.OtpTokens
                .FirstOrDefaultAsync(t =>
                    t.Email == email &&
                    t.Code == code &&
                    !t.IsUsed &&
                    t.ExpiresAt > DateTime.UtcNow);

            if (otpToken == null)
            {
                throw new Exception("Invalid, expired, or already used OTP code.");
            }

            otpToken.IsUsed = true;

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (user != null)
            {
                if (await IsUserBlacklistedAsync(user.Id))
                    throw new Exception("Your account has been banned.");
            }

            bool isNewUser = user == null;

            if (isNewUser)
            {
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    Name = email.Split('@')[0],
                    RoleId = 2,
                    CreatedAt = DateTime.UtcNow,
                };
                var defaultRole = await _context.UserRoles.FindAsync(2); // Find the default role
                user.Role = defaultRole;

                _context.Users.Add(user);
            }

            await _context.SaveChangesAsync();

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
                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                IsNewUser = isNewUser
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
