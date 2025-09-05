using DisasterReport.Services.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services
{
    public interface IOrganizationService
    {
        Task<int> GetActiveOrganizationCountAsync();
        Task<int> CreateOrganizationAsync(CreateOrganizationDto dto, Guid creatorUserId, string? logoUrl);
        Task<IEnumerable<OrganizationDto>> GetAllAsync();
        Task<IEnumerable<OrganizationDto>> GetPendingOrgsAsync();
        Task<IEnumerable<OrganizationDto>> GetRejectedOrgsAsync();
        Task<OrganizationDto?> GetByIdAsync(int id);
        Task<OrganizationDto?> GetDetailsByIdAsync(int id);
        Task<bool> ApproveOrganizationAsync(int orgId, Guid adminUserId);
        Task<bool> RejectOrganizationAsync(int orgId, Guid adminUserId);
        Task<bool> UpdateOrganizationAsync(UpdateOrganizationDto dto);
        Task<IEnumerable<OrganizationDto>> GetBlacklistedOrgsAsync();
        Task<bool> BlacklistOrganizationAsync(int orgId, Guid adminUserId);
        Task<bool> UnBlacklistOrganizationAsync(int orgId, Guid adminUserId);
        Task<bool> InviteMemberAsync(int orgId, InviteMemberDto dto, Guid inviterUserId);
        Task<bool> UserHasActiveOrganizationAsync(Guid userId);
        Task<string?> UpdateLogoAsync(int orgId, IFormFile logoFile, Guid userId);
    }
}
