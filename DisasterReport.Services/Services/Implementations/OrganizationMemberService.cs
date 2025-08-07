using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Enums;
using DisasterReport.Services.Models;

namespace DisasterReport.Services.Services
{
    public class OrganizationMemberService : IOrganizationMemberService
    {
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IOrganizationMemberRepo _orgMemberRepo;
        private readonly IUserRepo _userRepo;
        private readonly InvitationNotificationService _invitationNotificationService;

        public OrganizationMemberService(
            IOrganizationRepo organizationRepo,
            IOrganizationMemberRepo orgMemberRepo,
            IUserRepo userRepo,
            InvitationNotificationService invitationNotificationService)
        {
            _organizationRepo = organizationRepo;
            _orgMemberRepo = orgMemberRepo;
            _userRepo = userRepo;
            _invitationNotificationService = invitationNotificationService;
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

            // Update member entry
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

            // Since one user has only one organization, get the first or default
            var orgMember = member.FirstOrDefault();

            if (orgMember == null || orgMember.Organization == null)
                return null;

            return new UserOrganizationDto
            {
                OrganizationId = orgMember.OrganizationId,
                OrganizationName = orgMember.Organization.Name,
                RoleInOrg = orgMember.RoleInOrg
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

            // Create invitation entity
            var invitation = new OrganizationMember
            {
                OrganizationId = organizationId,
                UserId = invitedUser?.Id,  // nullable UserId property expected
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

            // TODO: Optionally, send email invite for unregistered users here

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
    }
}
