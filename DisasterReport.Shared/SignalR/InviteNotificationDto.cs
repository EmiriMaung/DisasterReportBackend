
namespace DisasterReport.Shared.SignalR
{
    public class InviteNotificationDto
    {
        public string OrganizationName { get; set; } = string.Empty;
        public string RoleInOrg { get; set; } = string.Empty;
        public string InvitedEmail { get; set; } = string.Empty;
    }
}
