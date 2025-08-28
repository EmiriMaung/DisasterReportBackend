namespace DisasterReport.Services.Models.AuthDTO
{
    public class TokenResultDto
    {
        public string AccessToken { get; set; } = null!;
        public string? RefreshToken { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsNewUser { get; set; }
    }
}
