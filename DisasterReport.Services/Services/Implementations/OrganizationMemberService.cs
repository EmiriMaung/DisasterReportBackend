using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Enums;
using DisasterReport.Services.Models;
using DisasterReport.Services.Services.Interfaces;

namespace DisasterReport.Services.Services
{
    public class OrganizationMemberService : IOrganizationMemberService
    {
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IOrganizationMemberRepo _orgMemberRepo;
        private readonly IUserRepo _userRepo;
        private readonly InvitationNotificationService _invitationNotificationService;
        private readonly IEmailServices _emailService;

        public OrganizationMemberService(
            IOrganizationRepo organizationRepo,
            IOrganizationMemberRepo orgMemberRepo,
            IUserRepo userRepo,
            InvitationNotificationService invitationNotificationService,
            IEmailServices emailService
            )
        {
            _organizationRepo = organizationRepo;
            _orgMemberRepo = orgMemberRepo;
            _userRepo = userRepo;
            _invitationNotificationService = invitationNotificationService;
            _emailService = emailService;
        }

        public async Task<bool> AcceptInvitationAsync(AcceptInvitationDto dto, Guid userId)
        {
            var member = await _orgMemberRepo.GetByTokenAsync(dto.Token);
            if (member == null)
                throw new InvalidOperationException("Invalid or expired invitation token.");

            if (member.IsAccepted)
                throw new InvalidOperationException("Invitation already accepted.");

            if (member.UserId != null && member.UserId != userId)
                throw new InvalidOperationException("Invitation was not intended for you.");

            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found.");

            member.UserId = userId;
            member.IsAccepted = true;
            member.JoinedAt = DateTime.UtcNow;

            return await _orgMemberRepo.SaveChangesAsync();
        }

        public async Task<List<OrganizationMemberDto>> GetMembersByOrganizationIdAsync(int organizationId)
        {
            var members = await _orgMemberRepo.GetByOrganizationIdAsync(organizationId);

            return members.Select(m => new OrganizationMemberDto
            {
                UserId = m.UserId,
                InvitedEmail = m.User?.Email ?? m.InvitedEmail,
                RoleInOrg = m.RoleInOrg,
                JoinedAt = m.JoinedAt,
                IsAccepted = m.IsAccepted
            }).ToList();
        }

        public async Task<UserOrganizationDto?> GetUserOrganizationAsync(Guid userId)
        {
            var member = await _orgMemberRepo.GetUserOrganizationsWithOrgAsync(userId);

            var orgMember = member.FirstOrDefault();

            if (orgMember == null || orgMember.Organization == null)
                return null;

            return new UserOrganizationDto
            {
                OrganizationId = orgMember.OrganizationId,
                OrganizationName = orgMember.Organization.Name,
                RoleInOrg = orgMember.RoleInOrg,
                Status = orgMember.Organization.Status
            };
        }

        public async Task<bool> InviteMemberAsync(int organizationId, InviteMemberDto dto, Guid ownerUserId)
        {
            // Check organization exists and is approved
            var org = await _organizationRepo.GetByIdAsync(organizationId);
            if (org == null || org.Status != (int)Status.Approved)
                throw new InvalidOperationException("Organization not found or not approved.");

            // Check inviter is owner
            var ownerMember = await _orgMemberRepo.GetByOrgAndUserAsync(organizationId, ownerUserId);
            if (ownerMember == null || ownerMember.RoleInOrg?.ToLower() != "owner")
                throw new UnauthorizedAccessException("Only owner can invite members.");

            // Check if invited user exists
            var invitedUser = await _userRepo.GetUsersByEmailAsync(dto.InvitedEmail);
            if (invitedUser != null)
            {
                var existingMember = await _orgMemberRepo.GetByOrgAndUserAsync(organizationId, invitedUser.Id);
                if (existingMember != null)
                    throw new InvalidOperationException("User is already a member.");
            }

            var invitation = new OrganizationMember
            {
                OrganizationId = organizationId,
                UserId = invitedUser?.Id,  
                RoleInOrg = dto.RoleInOrg ?? "Member",
                InvitedEmail = dto.InvitedEmail,
                IsAccepted = false,
                InvitationToken = Guid.NewGuid(),
                JoinedAt = null
            };

            await _orgMemberRepo.AddAsync(invitation);
            var saved = await _orgMemberRepo.SaveChangesAsync();

            if (saved && invitedUser != null)
            {
                // Send SignalR notification
                await _invitationNotificationService.NotifyUserAsync(
                    invitedUser.Id.ToString(),
                    org.Name,
                    ownerMember.UserId.ToString());
            }

            var subject = $"You're invited to join {org.Name}";
            var body = $@"
            <html>
            <body>
                <p>Hello</p>
                <p>You have been invited to join the organization '<strong>{org.Name}</strong>' as a '<strong>{invitation.RoleInOrg}</strong>'.</p>
                <p>Please login to your account to Civic Responders platform to accept the request.</p>
            </body>
            </html>";

            await _emailService.SendEmailAsync(dto.InvitedEmail, subject, body);

            return saved;
        }

        public async Task<bool> RemoveMemberAsync(int organizationId, Guid userId)
        {
            var member = await _orgMemberRepo.GetByOrgAndUserAsync(organizationId, userId);
            if (member == null)
                throw new InvalidOperationException("Member not found in this organization.");

            await _orgMemberRepo.RemoveAsync(member);
            return await _orgMemberRepo.SaveChangesAsync();
        }
        public async Task<bool> RejectInvitationAsync(Guid token, Guid userId)
        {
            var member = await _orgMemberRepo.GetByTokenAsync(token);
            if (member == null)
                throw new InvalidOperationException("Invalid or expired invitation token.");

            if (member.IsAccepted)
                throw new InvalidOperationException("Cannot reject an already accepted invitation.");

            if (member.UserId != null && member.UserId != userId)
                throw new InvalidOperationException("This invitation was not intended for you.");

            await _orgMemberRepo.RemoveAsync(member);

            return await _orgMemberRepo.SaveChangesAsync();
        }
        public async Task<List<PendingInvitationDto>> GetPendingInvitationsAsync(Guid userId)
        {
            var invitations = await _orgMemberRepo.GetUserOrganizationsWithOrgAsync(userId);

            return invitations
                .Where(i => !i.IsAccepted) // only pending
                .Select(i => new PendingInvitationDto
                {
                    OrganizationId = i.OrganizationId,
                    OrganizationName = i.Organization?.Name ?? "",
                    Token = (Guid)i.InvitationToken
                })
                .ToList();
        }
    }
}
