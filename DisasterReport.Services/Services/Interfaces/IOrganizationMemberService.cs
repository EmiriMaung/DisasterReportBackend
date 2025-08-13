using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DisasterReport.Services.Models;

namespace DisasterReport.Services.Services
{
    public interface IOrganizationMemberService
    {
        Task<bool> InviteMemberAsync(int organizationId, InviteMemberDto dto, Guid ownerUserId);
        Task<bool> AcceptInvitationAsync(AcceptInvitationDto dto, Guid userId);
        Task<bool> RemoveMemberAsync(int organizationId, Guid userId);
        Task<List<OrganizationMemberDto>> GetMembersByOrganizationIdAsync(int organizationId);
        Task<UserOrganizationDto?> GetUserOrganizationAsync(Guid userId);
        Task<bool> RejectInvitationAsync(Guid token, Guid userId); //new method
        Task<List<PendingInvitationDto>> GetPendingInvitationsAsync(Guid userId);
    }
}
