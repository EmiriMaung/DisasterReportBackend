using DisasterReport.Services.Models;
using DisasterReport.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace DisasterReport.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _organizationService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _organizationService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var result = await _organizationService.GetDetailsByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpGet("active-count")]
        [AllowAnonymous]
        public async Task<int> GetActiveOrganizationCountAsync()
        {
            return await _organizationService.GetActiveOrganizationCountAsync();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromForm] CreateOrganizationDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid creatorUserId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            try
            {

                var newOrgId = await _organizationService.CreateOrganizationAsync(dto, creatorUserId,null);
                return CreatedAtAction(nameof(GetById), new { id = newOrgId }, new { Id = newOrgId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> Update(
        int id,
        [FromForm] UpdateOrganizationDto dto)
            {
                if (id != dto.Id)
                    return BadRequest("Mismatched ID");

                var success = await _organizationService.UpdateOrganizationAsync(dto);
                if (!success)
                    return NotFound();

                return NoContent();
            }

        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); 
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid adminUserId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            var success = await _organizationService.ApproveOrganizationAsync(id, adminUserId);
            if (!success)
                return NotFound();

            return Ok("Organization approved.");
        }

        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid adminUserId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            var success = await _organizationService.RejectOrganizationAsync(id, adminUserId);
            if (!success)
                return NotFound();

            return Ok("Organization rejected.");
        }

        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingOrganizations()
        {
            var result = await _organizationService.GetPendingOrgsAsync();
            return Ok(result);
        }

        [HttpGet("rejected")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRejectedOrganizations()
        {
            var result = await _organizationService.GetRejectedOrgsAsync();
            return Ok(result);
        }

        [HttpGet("blacklisted")]
        public async Task<IActionResult> GetBlacklisted()
        {
            var result = await _organizationService.GetBlacklistedOrgsAsync();
            return Ok(result);
        }

        [HttpPost("{id}/blacklist")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Blacklist(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); 
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid adminUserId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            var success = await _organizationService.BlacklistOrganizationAsync(id, adminUserId);
            if (!success)
                return NotFound();

            return Ok("Organization blacklisted.");
        }

        [HttpPost("{id}/unblacklist")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnBlacklist(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); 
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid adminUserId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            var success = await _organizationService.UnBlacklistOrganizationAsync(id, adminUserId);
            if (!success)
                return NotFound();

            return Ok("Organization removed from blacklist.");
        }
        
        [HttpPut("{id}/logo")]
        [Authorize]
        public async Task<IActionResult> UpdateLogo(int id, IFormFile logo)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            var updatedLogoUrl = await _organizationService.UpdateLogoAsync(id, logo, userId);
            if (updatedLogoUrl == null)
                return NotFound("Organization not found");

            return Ok(new { LogoUrl = updatedLogoUrl });
        }

    }
}
