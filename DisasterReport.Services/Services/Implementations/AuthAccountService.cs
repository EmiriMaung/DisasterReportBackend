using DisasterReport.Data.Domain;
using DisasterReport.Services.Models.AuthDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DisasterReport.Services.Services.Implementations
{
    public class AuthAccountService : IAuthAccountService
    {
        private readonly ApplicationDBContext _context;
        private readonly IJwtService _jwtService;
        private readonly IConfiguration _config;

        public AuthAccountService(ApplicationDBContext context, IJwtService jwtService, IConfiguration config)
        {
            _context = context;
            _jwtService = jwtService;
            _config = config; // Assign injected IConfiguration
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
                // Existing login found
                user = login.User;
            }
            else
            {
                // Check if the user already exists by email
                user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == userInfo.Email);

                if (user == null)
                {
                    var adminEmails = _config["Admin:Email"]
                    ?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    ?? Array.Empty<string>();

                    var isAdmin = adminEmails.Contains(userInfo.Email, StringComparer.OrdinalIgnoreCase);


                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = userInfo.Email,
                        Name = userInfo.Name,
                        RoleId = isAdmin ? 1 : 2, // 1 = Admin, 2 = User
                        ProfilePictureUrl = userInfo.ProfilePictureUrl,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    user.Role = await _context.UserRoles.FindAsync(user.RoleId);
                }

                // Add external login for new or existing user
                var externalLogin = new ExternalLogin
                {
                    Provider = userInfo.Provider,
                    ProviderKey = userInfo.ProviderKey,
                    UserId = user.Id
                };

                _context.ExternalLogins.Add(externalLogin);
                await _context.SaveChangesAsync();
            }

            // Generate JWTs
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
    }
}
