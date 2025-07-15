using DisasterReport.Services.Models.AuthDTO;
using DisasterReport.Services.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace DisasterReport.Services.Services.Implementations
{
    public class OAuthService : IOAuthService
    {
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;

        public OAuthService(IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _config = config;
            _httpClientFactory = httpClientFactory;
        }
        public string GetLoginUrl(string provider, string state)
        {
            if (provider == "google")
            {
                var clientId = _config["Authentication:Google:ClientId"];
                var redirectUri = _config["Authentication:Google:RedirectUri"];
                var scope = "openid email profile";

                var queryParams = new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["redirect_uri"] = redirectUri,
                    ["response_type"] = "code",
                    ["scope"] = scope,
                    ["access_type"] = "offline",
                    ["prompt"] = "consent",
                    ["state"] = state // Use the provided state for CSRF protection
                };

                return QueryHelpers.AddQueryString("https://accounts.google.com/o/oauth2/v2/auth", queryParams);
            }
            else if (provider == "facebook")
            {
                var clientId = _config["Authentication:Facebook:AppId"];
                var redirectUri = _config["Authentication:Facebook:RedirectUri"];
                var scope = "email public_profile";

                var queryParams = new Dictionary<string, string>
                {
                    ["client_id"] = clientId,
                    ["redirect_uri"] = redirectUri,
                    ["response_type"] = "code",
                    ["scope"] = scope,
                    ["state"] = state // Use the provided state for CSRF protection
                };

                return QueryHelpers.AddQueryString("https://www.facebook.com/v18.0/dialog/oauth", queryParams);
            }

            throw new NotSupportedException("Unsupported provider");
        }

        public async Task<OAuthUserInfoDto> HandleCallbackAsync(string provider, string code, string state)
        {
            var client = _httpClientFactory.CreateClient();

            if (provider == "google")
            {
                var tokenResponse = await client.PostAsync("https://oauth2.googleapis.com/token", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["code"] = code,
                    ["client_id"] = _config["Authentication:Google:ClientId"],
                    ["client_secret"] = _config["Authentication:Google:ClientSecret"],
                    ["redirect_uri"] = _config["Authentication:Google:RedirectUri"],
                    ["grant_type"] = "authorization_code"
                }));

                tokenResponse.EnsureSuccessStatusCode();
                var tokenJson = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
                var accessToken = tokenJson.GetProperty("access_token").GetString();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var userInfoRes = await client.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
                var userJson = await userInfoRes.Content.ReadFromJsonAsync<JsonElement>();

                return new OAuthUserInfoDto
                {
                    Email = userJson.GetProperty("email").GetString()!,
                    Name = userJson.GetProperty("name").GetString()!,
                    Provider = "google",
                    ProviderKey = userJson.GetProperty("id").GetString()!,
                    ProfilePictureUrl = userJson.TryGetProperty("picture", out var pic) ? pic.GetString() : null
                };
            }
            else if (provider == "facebook")
            {
                var tokenRes = await client.GetAsync("https://graph.facebook.com/v18.0/oauth/access_token" +
                    $"?client_id={_config["Authentication:Facebook:AppId"]}" +
                    $"&redirect_uri={_config["Authentication:Facebook:RedirectUri"]}" +
                    $"&client_secret={_config["Authentication:Facebook:AppSecret"]}" +
                    $"&code={code}");

                tokenRes.EnsureSuccessStatusCode();
                var tokenJson = await tokenRes.Content.ReadFromJsonAsync<JsonElement>();
                var accessToken = tokenJson.GetProperty("access_token").GetString();

                var userRes = await client.GetAsync($"https://graph.facebook.com/me?fields=id,name,email,picture.type(large)&access_token={accessToken}");
                var userJson = await userRes.Content.ReadFromJsonAsync<JsonElement>();

                return new OAuthUserInfoDto
                {
                    Email = userJson.GetProperty("email").GetString()!,
                    Name = userJson.GetProperty("name").GetString()!,
                    Provider = "facebook",
                    ProviderKey = userJson.GetProperty("id").GetString()!,
                    ProfilePictureUrl = userJson.GetProperty("picture").GetProperty("data").GetProperty("url").GetString()
                };
            }

            throw new NotSupportedException("Unsupported provider");
        }
    }
}
