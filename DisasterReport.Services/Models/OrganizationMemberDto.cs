using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models
{
    public class OrganizationMemberDto
    {
        public Guid? UserId { get; set; }  // Nullable for unregistered users
        public string? InvitedEmail { get; set; }
        public string? RoleInOrg { get; set; }
        public DateTime? JoinedAt { get; set; }
        public bool IsAccepted { get; set; }
    }
    public class InviteMemberDto
    {
        public string InvitedEmail { get; set; } = null!;
        public string? RoleInOrg { get; set; }
    }

    public class AcceptInvitationDto
    {
        public Guid Token { get; set; }
    }

    public class UserOrganizationDto
    {
        public int OrganizationId { get; set; }
        public string OrganizationName { get; set; } = null!;
        public string? RoleInOrg { get; set; }
        public int? Status { get; set; }
    }

}
