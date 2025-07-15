using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models
{
    public class OrganizationMemberDto
    {
        public Guid UserId { get; set; }
        public string? RoleInOrg { get; set; }
        public DateTime? JoinedAt { get; set; }
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
}
