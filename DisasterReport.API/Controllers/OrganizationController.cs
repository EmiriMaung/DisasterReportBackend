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
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }

        // GET: api/organization
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _organizationService.GetAllAsync();
            return Ok(result);
        }

        // GET: api/organization/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _organizationService.GetByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // GET: api/organization/{id}/details
        [HttpGet("{id}/details")]
        public async Task<IActionResult> GetDetails(int id)
        {
            var result = await _organizationService.GetDetailsByIdAsync(id);
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // POST: api/organization
        [HttpPost]
        public async Task<IActionResult> Create([FromForm] CreateOrganizationDto dto)
        {
            //var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            //return Ok(claims);

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // ← this matches your JWT
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid creatorUserId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            // TEMPORARY: skip JWT claim lookup
            //Guid creatorUserId = Guid.Parse("0A795CD5-C2AE-4840-BF6F-813661CC76EB");

            var newOrgId = await _organizationService.CreateOrganizationAsync(dto, creatorUserId);
            return CreatedAtAction(nameof(GetById), new { id = newOrgId }, new { Id = newOrgId });
        }

        // PUT: api/organization/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrganizationDto dto)
        {
            if (id != dto.Id)
                return BadRequest("Mismatched ID");

            var success = await _organizationService.UpdateOrganizationAsync(dto);
            if (!success)
                return NotFound();

            return NoContent();
        }

        // POST: api/organization/{id}/approve
        [HttpPost("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Approve(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // ← this matches your JWT
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid adminUserId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            //Temp
            //Guid adminUserId = Guid.Parse("7B6D7655-8BEC-48C6-AE97-6411F50EF8A7");

            var success = await _organizationService.ApproveOrganizationAsync(id, adminUserId);
            if (!success)
                return NotFound();

            return Ok("Organization approved.");
        }

        // POST: api/organization/{id}/reject
        [HttpPost("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Reject(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // ← this matches your JWT
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid adminUserId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            //Temp
            //Guid adminUserId = Guid.Parse("7B6D7655-8BEC-48C6-AE97-6411F50EF8A7");

            var success = await _organizationService.RejectOrganizationAsync(id, adminUserId);
            if (!success)
                return NotFound();

            return Ok("Organization rejected.");
        }

        // GET: api/organization/pending
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetPendingOrganizations()
        {
            var result = await _organizationService.GetPendingOrgsAsync();
            return Ok(result);
        }

        // GET: api/organization/rejected
        [HttpGet("rejected")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetRejectedOrganizations()
        {
            var result = await _organizationService.GetRejectedOrgsAsync();
            return Ok(result);
        }

        // GET: api/organization/blacklisted
        [HttpGet("blacklisted")]
        public async Task<IActionResult> GetBlacklisted()
        {
            var result = await _organizationService.GetBlacklistedOrgsAsync();
            return Ok(result);
        }

        // POST: api/organization/{id}/blacklist
        [HttpPost("{id}/blacklist")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Blacklist(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // ← this matches your JWT
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid adminUserId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            //Temp
            //Guid adminUserId = Guid.Parse("7B6D7655-8BEC-48C6-AE97-6411F50EF8A7");

            var success = await _organizationService.BlacklistOrganizationAsync(id, adminUserId);
            if (!success)
                return NotFound();

            return Ok("Organization blacklisted.");
        }

        // POST: api/organization/{id}/unblacklist
        [HttpPost("{id}/unblacklist")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UnBlacklist(int id)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier); // ← this matches your JWT
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid adminUserId))
            {
                return Unauthorized("User ID claim is missing or invalid");
            }

            //Temp
            //Guid adminUserId = Guid.Parse("7B6D7655-8BEC-48C6-AE97-6411F50EF8A7");

            var success = await _organizationService.UnBlacklistOrganizationAsync(id, adminUserId);
            if (!success)
                return NotFound();

            return Ok("Organization removed from blacklist.");
        }

    }
}
