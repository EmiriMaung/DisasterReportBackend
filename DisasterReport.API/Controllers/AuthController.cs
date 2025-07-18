using DisasterReport.Data.Domain;
using DisasterReport.Services.Models.AuthDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IOAuthService _oAuthService;
    private readonly IAuthAccountService _authAccountService;
    private readonly ApplicationDBContext _context;
    private readonly IJwtService _jwtService;
    private readonly IConfiguration _config;

    public AuthController(IOAuthService oAuthService, IAuthAccountService authAccountService, ApplicationDBContext context, IJwtService jwtService, IConfiguration config)
    {
        _oAuthService = oAuthService;
        _authAccountService = authAccountService;
        _context = context;
        _jwtService = jwtService;
        _config = config;
    }


    [HttpGet("login/{provider}")]
    public IActionResult Login(string provider)
    {
        var state = Guid.NewGuid().ToString();

        var loginUrl = _oAuthService.GetLoginUrl(provider.ToLower(), state);

        // Store the state in a secure, short-lived cookie
        Response.Cookies.Append("oauth_state", state, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(10)
        });

        return Redirect(loginUrl);
    }


    [HttpGet("callback/{provider}")]
    public async Task<IActionResult> Callback(string provider, [FromQuery] string? code, [FromQuery] string? state)
    {
        if (string.IsNullOrEmpty(code))
            return BadRequest(new { error = "Missing 'code' in query string." });

        var expectedState = Request.Cookies["oauth_state"];
        if (string.IsNullOrEmpty(state) || state != expectedState)
            return BadRequest(new { error = "Invalid or missing state parameter (possible CSRF)." });

        // Optional: clear state cookie after use
        Response.Cookies.Delete("oauth_state");

        var userInfo = await _oAuthService.HandleCallbackAsync(provider.ToLower(), code, state);
        var tokens = await _authAccountService.LoginOrRegisterExternalAsync(userInfo);

        // Set Access Token cookie (short-lived)
        Response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(10)
        });

        // Set Refresh Token cookie (longer lived)
        Response.Cookies.Append("refresh_token", tokens.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return Redirect($"{_config["ClientUrl"]}/oauth-callback");
        //return Ok("Login successful. You can close this window now.");
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        try
        {
            var tokens = await _authAccountService.RegisterAsync(dto);

            // Set cookies if needed (optional)
            Response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = tokens.ExpiresAt
            });

            Response.Cookies.Append("refresh_token", tokens.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });

            return Ok(tokens);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var tokens = await _authAccountService.LoginAsync(dto);

            // Optional: Set cookies
            Response.Cookies.Append("access_token", tokens.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = tokens.ExpiresAt
            });

            Response.Cookies.Append("refresh_token", tokens.RefreshToken!, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(30)
            });

            return Ok(tokens);
        }
        catch (Exception ex)
        {
            return Unauthorized(new { error = ex.Message });
        }
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
        {
            var token = await _context.RefreshTokens.FirstOrDefaultAsync(t => t.Token == refreshToken);
            if (token != null)
            {
                token.RevokedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        Response.Cookies.Delete("access_token");
        Response.Cookies.Delete("refresh_token");

        return Ok(new { message = "Logged out successfully." });
    }


    [Authorize]
    [HttpGet("me")]
    public IActionResult Me()
    {
        return Ok(new
        {
            id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            email = User.FindFirst(ClaimTypes.Email)?.Value,
            name = User.FindFirst("name")?.Value,
            role = User.FindFirst(ClaimTypes.Role)?.Value,
            profilePicture = User.FindFirst("profilePicture")?.Value
        });
    }


    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken()
    {
        if (!Request.Cookies.TryGetValue("refresh_token", out var refreshToken))
            return Unauthorized(new { error = "Refresh token missing." });

        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
            .ThenInclude(u => u.Role)
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken);

        if (token == null || token.IsUsed || token.RevokedAt != null || token.ExpiresAt < DateTime.UtcNow)
            return Unauthorized(new { error = "Invalid or expired refresh token." });

        // Mark the old refresh token as used/revoked
        token.IsUsed = true;
        token.RevokedAt = DateTime.UtcNow;

        var newAccessToken = _jwtService.GenerateAccessToken(token.User);
        var newRefreshToken = _jwtService.GenerateRefreshToken();

        // Save the new refresh token
        var newTokenEntity = new RefreshToken
        {
            Token = newRefreshToken,
            UserId = token.UserId,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            CreatedAt = DateTime.UtcNow
        };

        token.ReplacedByToken = newRefreshToken;

        _context.RefreshTokens.Add(newTokenEntity);
        await _context.SaveChangesAsync();

        // Set new refresh token cookie
        Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddDays(30)
        });

        return Ok(new TokenResultDto
        {
            AccessToken = newAccessToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10)
        });
    }
}