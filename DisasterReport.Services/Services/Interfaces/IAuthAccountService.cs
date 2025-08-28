using DisasterReport.Services.Models.AuthDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services.Interfaces
{
    public interface IAuthAccountService
    {
        Task<TokenResultDto> LoginOrRegisterExternalAsync(OAuthUserInfoDto userInfo);

        Task RequestOtpAsync(string email);

        Task<TokenResultDto> AuthenticateWithOtpAsync(string email, string code);

        //Task<TokenResultDto> RegisterAsync(RegisterDto registerDto);

        //public Task<TokenResultDto> LoginAsync(LoginDto dto);
    }
}
