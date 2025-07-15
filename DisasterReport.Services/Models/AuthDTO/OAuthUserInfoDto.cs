namespace DisasterReport.Services.Models.AuthDTO
{
    public class OAuthUserInfoDto
    {
        public string Email { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Provider { get; set; } = null!;
        public string ProviderKey { get; set; } = null!;
        public string? ProfilePictureUrl { get; set; }
    }
}
