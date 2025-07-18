using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
using DisasterReport.Services.Enums;
using DisasterReport.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IOrganizationDocRepo _organizationDocRepo;
        private readonly IOrganizationMemberRepo _organizationMemberRepo;
        private readonly ICloudinaryService _cloudinaryService;

        public OrganizationService(
            IOrganizationRepo organizationRepo,
            IOrganizationDocRepo organizationDocRepo,
            IOrganizationMemberRepo organizationMemberRepo,
            ICloudinaryService cloudinaryService)
        {
            _organizationRepo = organizationRepo;
            _organizationDocRepo = organizationDocRepo;
            _organizationMemberRepo = organizationMemberRepo;
            _cloudinaryService = cloudinaryService;
        }
        public async Task<bool> ApproveOrganizationAsync(int orgId, Guid adminUserId)
        {
            var org = await _organizationRepo.GetByIdAsync(orgId);
            if (org == null)
                return false;

            org.Status = (int)Status.Approved;
            org.ApprovedBy = adminUserId;
            org.ApprovedAt = DateTime.UtcNow;

            _organizationRepo.Update(org);
            return await _organizationRepo.SaveChangesAsync();
        }

        public async Task<int> CreateOrganizationAsync(CreateOrganizationDto dto, Guid creatorUserId)
        {
            // ✅ Check if user already has an active (non-rejected) organization
            if (await UserHasActiveOrganizationAsync(creatorUserId))
            {
                throw new InvalidOperationException("You already have an active organization (pending, approved, or blacklisted).");
            }

            var organization = new Organization
            {
                Name = dto.Name,
                OrganizationEmail = dto.OrganizationEmail,
                Description = dto.Description,
                PhoneNumber = dto.PhoneNumber,
                Status = (int)Status.Pending,
                IsBlackListedOrg = false
            };

            // Add Organization entity
            await _organizationRepo.AddAsync(organization);
            await _organizationRepo.SaveChangesAsync(); // Save to get organization.Id

            // Upload docs and save OrganizationDoc entities
            foreach (var file in dto.Documents)
            {
                var uploadResult = await _cloudinaryService.UploadFileAsync(file);
                var orgDoc = new OrganizationDoc
                {
                    OrganizationId = organization.Id,
                    ImageUrl = uploadResult.SecureUrl,
                    FileName = uploadResult.FileName,
                    FileType = uploadResult.FileType,
                    CreatedAt = DateTime.UtcNow
                };

                await _organizationDocRepo.AddAsync(orgDoc);
            }
            await _organizationDocRepo.SaveChangesAsync();

            // Add creator as Owner in OrganizationMembers
            var ownerMember = new OrganizationMember
            {
                OrganizationId = organization.Id,
                UserId = creatorUserId,
                RoleInOrg = "Owner",
                JoinedAt = DateTime.UtcNow
            };
            await _organizationMemberRepo.AddAsync(ownerMember);
            await _organizationMemberRepo.SaveChangesAsync();

            return organization.Id;
        }

        public async Task<IEnumerable<OrganizationDto>> GetAllAsync()
        {
            var orgs = await _organizationRepo.GetAllAsync(); // Summary only

            var dtoList = orgs.Select(org => new OrganizationDto
            {
                Id = org.Id,
                Name = org.Name,
                OrganizationEmail = org.OrganizationEmail,
                Description = org.Description,
                PhoneNumber = org.PhoneNumber,
                IsBlackListedOrg = org.IsBlackListedOrg,
                Status = (Status)org.Status,
                ApprovedBy = org.ApprovedBy,
                ApprovedAt = org.ApprovedAt,
                Docs = null,   // No detailed docs here
                Members = null // No detailed members here
            }).ToList();

            return dtoList;
        }
        public async Task<IEnumerable<OrganizationDto>> GetPendingOrgsAsync()
        {
            var orgs = await _organizationRepo.GetPendingOrgsAsync();

            var dtoList = orgs.Select(org => new OrganizationDto
            {
                Id = org.Id,
                Name = org.Name,
                OrganizationEmail = org.OrganizationEmail,
                Description = org.Description,
                PhoneNumber = org.PhoneNumber,
                IsBlackListedOrg = org.IsBlackListedOrg,
                Status = (Status)org.Status,
                ApprovedBy = org.ApprovedBy,
                ApprovedAt = org.ApprovedAt,
                Docs = null,
                Members = null
            }).ToList();

            return dtoList;
        }
        public async Task<IEnumerable<OrganizationDto>> GetRejectedOrgsAsync()
        {
            var orgs = await _organizationRepo.GetRejectedOrgsAsync();

            var dtoList = orgs.Select(org => new OrganizationDto
            {
                Id = org.Id,
                Name = org.Name,
                OrganizationEmail = org.OrganizationEmail,
                Description = org.Description,
                PhoneNumber = org.PhoneNumber,
                IsBlackListedOrg = org.IsBlackListedOrg,
                Status = (Status)org.Status,
                ApprovedBy = org.ApprovedBy,
                ApprovedAt = org.ApprovedAt,
                Docs = null,
                Members = null
            }).ToList();

            return dtoList;
        }
        public async Task<OrganizationDto?> GetByIdAsync(int id)
        {
            var org = await _organizationRepo.GetByIdAsync(id); // Summary only

            if (org == null)
                return null;

            var dto = new OrganizationDto
            {
                Id = org.Id,
                Name = org.Name,
                OrganizationEmail = org.OrganizationEmail,
                Description = org.Description,
                PhoneNumber = org.PhoneNumber,
                IsBlackListedOrg = org.IsBlackListedOrg,
                Status = (Status)org.Status,
                ApprovedBy = org.ApprovedBy,
                ApprovedAt = org.ApprovedAt,
                Docs = null,
                Members = null
            };

            return dto;
        }

        public async Task<OrganizationDto?> GetDetailsByIdAsync(int id)
        {
            var org = await _organizationRepo.GetDetailsByIdAsync(id);
            if (org == null)
                return null;
            var dto = new OrganizationDto
            {
                Id = org.Id,
                Name = org.Name,
                OrganizationEmail = org.OrganizationEmail,
                Description = org.Description,
                PhoneNumber = org.PhoneNumber,
                IsBlackListedOrg = org.IsBlackListedOrg,
                Status = (Status)org.Status,
                ApprovedBy = org.ApprovedBy,
                ApprovedAt = org.ApprovedAt,
                Docs = new List<OrganizationDocDto>(),
                Members = new List<OrganizationMemberDto>()
            };
            foreach (var doc in org.OrganizationDocs)
            {
                dto.Docs.Add(new OrganizationDocDto
                {
                    ImageUrl = doc.ImageUrl,
                    FileName = doc.FileName,
                    FileType = doc.FileType,
                    CreatedAt = doc.CreatedAt
                });
            }
            foreach (var member in org.OrganizationMembers)
            {
                dto.Members.Add(new OrganizationMemberDto
                {
                    UserId = member.UserId,
                    RoleInOrg = member.RoleInOrg,
                    JoinedAt = member.JoinedAt
                });
            }
            return dto;
        }
        public async Task<bool> RejectOrganizationAsync(int orgId, Guid adminUserId)
        {
            var org = await _organizationRepo.GetByIdAsync(orgId);
            if (org == null)
                return false;

            org.Status = (int)Status.Rejected;
            org.ApprovedBy = adminUserId;
            org.ApprovedAt = DateTime.UtcNow;

            _organizationRepo.Update(org);
            return await _organizationRepo.SaveChangesAsync();
        }

        public async Task<bool> UpdateOrganizationAsync(UpdateOrganizationDto dto)
        {
            var org = await _organizationRepo.GetByIdAsync(dto.Id);
            if (org == null)
                return false;

            org.Name = dto.Name;
            org.OrganizationEmail = dto.OrganizationEmail;
            org.Description = dto.Description;
            org.PhoneNumber = dto.PhoneNumber;

            _organizationRepo.Update(org);
            return await _organizationRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<OrganizationDto>> GetBlacklistedOrgsAsync()
        {
            var orgs = await _organizationRepo.GetAllBlackListedOrgsAsync();

            var dtoList = new List<OrganizationDto>();
            foreach (var org in orgs)
            {
                var dto = new OrganizationDto
                {
                    Id = org.Id,
                    Name = org.Name,
                    OrganizationEmail = org.OrganizationEmail,
                    Description = org.Description,
                    PhoneNumber = org.PhoneNumber,
                    IsBlackListedOrg = org.IsBlackListedOrg,
                    Status = (Status)org.Status,
                    ApprovedBy = org.ApprovedBy,
                    ApprovedAt = org.ApprovedAt,
                    Docs = new List<OrganizationDocDto>(),
                    Members = new List<OrganizationMemberDto>()
                };
                dtoList.Add(dto);
            }

            return dtoList;
        }

        public async Task<bool> BlacklistOrganizationAsync(int orgId, Guid adminUserId)
        {
            var org = await _organizationRepo.GetByIdAsync(orgId);
            if (org == null)
                return false;

            org.IsBlackListedOrg = true;
            org.ApprovedBy = adminUserId;
            org.ApprovedAt = DateTime.UtcNow;

            _organizationRepo.Update(org);
            return await _organizationRepo.SaveChangesAsync();
        }

        public async Task<bool> UnBlacklistOrganizationAsync(int orgId, Guid adminUserId)
        {
            var org = await _organizationRepo.GetByIdAsync(orgId);
            if (org == null)
                return false;

            org.IsBlackListedOrg = false;
            org.ApprovedBy = adminUserId;
            org.ApprovedAt = DateTime.UtcNow;

            _organizationRepo.Update(org);
            return await _organizationRepo.SaveChangesAsync();
        }

        public Task<bool> InviteMemberAsync(int orgId, InviteMemberDto dto, Guid inviterUserId)
        {
            throw new NotImplementedException();
        }
        public async Task<bool> UserHasActiveOrganizationAsync(Guid userId)
        {
            var organizations = await _organizationMemberRepo.GetUserOrganizationsWithOrgAsync(userId);

            return organizations.Any(m => m.Organization.Status != (int)Status.Rejected);
        }
    }
}
