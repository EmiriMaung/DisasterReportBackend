using DisasterReport.Services.Models.AuthDTO;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IOAuthService
    {
        string GetLoginUrl(string provider, string state);
        Task<OAuthUserInfoDto> HandleCallbackAsync(string provider, string code, string state);
    }
}
