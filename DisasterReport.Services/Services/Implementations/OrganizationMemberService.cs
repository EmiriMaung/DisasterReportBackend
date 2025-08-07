using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Enums;
using DisasterReport.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services
{
    public class OrganizationMemberService : IOrganizationMemberService
    {
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IOrganizationMemberRepo _orgMemberRepo;
        private readonly IUserRepo _userRepo;

        public OrganizationMemberService(
            IOrganizationRepo organizationRepo,
            IOrganizationMemberRepo orgMemberRepo,
            IUserRepo userRepo)
        {
            _organizationRepo = organizationRepo;
            _orgMemberRepo = orgMemberRepo;
            _userRepo = userRepo;
        }

        public async Task<bool> AcceptInvitationAsync(AcceptInvitationDto dto, Guid userId)
        {
            var member = await _orgMemberRepo.GetByTokenAsync(dto.Token);
            if (member == null)
                throw new Exception("Invalid or expired invitation token.");

            if (member.IsAccepted)
                throw new Exception("Invitation already accepted.");

            if (member.UserId != null && member.UserId != userId)
                throw new Exception("Invitation was not intended for you.");

            var user = await _userRepo.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found.");

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
                throw new Exception("Organization not found or not approved.");

            // Check inviter is owner
            var ownerMember = await _orgMemberRepo.GetByOrgAndUserAsync(organizationId, ownerUserId);
            if (ownerMember == null || ownerMember.RoleInOrg?.ToLower() != "owner")
                throw new Exception("Only owner can invite members.");

            // Check if invited user exists
            var invitedUser = await _userRepo.GetUsersByEmailAsync(dto.InvitedEmail);
            if (invitedUser != null)
            {
                var existingMember = await _orgMemberRepo.GetByOrgAndUserAsync(organizationId, invitedUser.Id);
                if (existingMember != null)
                    throw new Exception("User is already a member.");
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

            // TODO: Send notification or email invite here

            return saved;
        }

        public async Task<bool> RemoveMemberAsync(int organizationId, Guid userId)
        {
            var member = await _orgMemberRepo.GetByOrgAndUserAsync(organizationId, userId);
            if (member == null)
                throw new Exception("Member not found in this organization.");

            await _orgMemberRepo.RemoveAsync(member);
            return await _orgMemberRepo.SaveChangesAsync();
        }
        
    }
}
