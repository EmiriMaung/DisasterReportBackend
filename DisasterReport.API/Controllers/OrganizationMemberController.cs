using DisasterReport.Services.Models;
using DisasterReport.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace DisasterReport.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationMemberController : ControllerBase
    {
        private readonly IOrganizationMemberService _memberService;

        public OrganizationMemberController(IOrganizationMemberService memberService)
        {
            _memberService = memberService;
        }

        // ✅ Invite a member (POST)
        [HttpPost("{organizationId}/invite")]
        [Authorize]
        public async Task<IActionResult> InviteMember(int organizationId, [FromBody] InviteMemberDto dto)
        {
            var userId = GetUserId(); // from token

            var success = await _memberService.InviteMemberAsync(organizationId, dto, userId);
            if (!success)
                return BadRequest("Failed to invite member.");

            return Ok("Invitation sent.");
        }

        // ✅ Accept invitation (POST)
        [HttpPost("accept")]
        [Authorize]
        public async Task<IActionResult> AcceptInvitation([FromBody] AcceptInvitationDto dto)
        {
            var userId = GetUserId();

            var success = await _memberService.AcceptInvitationAsync(dto, userId);
            if (!success)
                return BadRequest("Failed to accept invitation.");

            return Ok("Invitation accepted.");
        }

        // ✅ Remove a member (DELETE)
        [HttpDelete("{organizationId}/remove/{userId}")]
        [Authorize]
        public async Task<IActionResult> RemoveMember(int organizationId, Guid userId)
        {
            var success = await _memberService.RemoveMemberAsync(organizationId, userId);
            if (!success)
                return BadRequest("Failed to remove member.");

            return Ok("Member removed.");
        }

        // ✅ Get all members in an organization (GET)
        [HttpGet("{organizationId}/members")]
        public async Task<IActionResult> GetMembers(int organizationId)
        {
            var members = await _memberService.GetMembersByOrganizationIdAsync(organizationId);
            return Ok(members);
        }

        [HttpGet("user/{userId}/organization")]
        public async Task<IActionResult> GetUserOrganization(Guid userId)
        {
            var org = await _memberService.GetUserOrganizationAsync(userId);
            if (org == null) return NotFound();
            return Ok(org);
        }

        // POST: /api/OrganizationMember/reject
        [HttpPost("reject")]
        [Authorize]
        public async Task<IActionResult> RejectInvitation([FromBody] AcceptInvitationDto dto)
        {
            var userId = GetUserId(); // from JWT

            var success = await _memberService.RejectInvitationAsync(dto.Token, userId);
            if (!success)
                return BadRequest("Failed to reject invitation.");

            return Ok("Invitation rejected.");
        }
        // GET: /api/OrganizationMember/user/{userId}/pending-invitations
        [HttpGet("user/{userId}/pending-invitations")]
        [Authorize]
        public async Task<IActionResult> GetPendingInvitations(Guid userId)
        {
            var invitations = await _memberService.GetPendingInvitationsAsync(userId);
            return Ok(invitations);
        }


        // Utility to extract user ID from JWT
        private Guid GetUserId()
        {
            var userIdStr = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            return Guid.Parse(userIdStr!);
        }
    }
}
