namespace DisasterReport.Services.Models.UserDTO
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string RoleName { get; set; } = null!;

        public string? ProfilePictureUrl { get; set; }

        public bool IsBlacklistedUser { get; set; }

        public DateTime? CreatedAt { get; set; }

        public List<string> OrganizationNames { get; set; } = new List<string>();
    }
}
