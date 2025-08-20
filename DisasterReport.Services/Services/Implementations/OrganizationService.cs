using DisasterReport.Data.Domain;
using DisasterReport.Data.Repositories;
using DisasterReport.Data.Repositories.Interfaces;
using DisasterReport.Services.Enums;
using DisasterReport.Services.Models;
using Microsoft.AspNetCore.Http;

namespace DisasterReport.Services.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IOrganizationRepo _organizationRepo;
        private readonly IOrganizationDocRepo _organizationDocRepo;
        private readonly IOrganizationMemberRepo _organizationMemberRepo;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IUserRepo _userRepo;

        public OrganizationService(
            IOrganizationRepo organizationRepo,
            IOrganizationDocRepo organizationDocRepo,
            IOrganizationMemberRepo organizationMemberRepo,
            ICloudinaryService cloudinaryService,
            IUserRepo userRepo)
        {
            _organizationRepo = organizationRepo;
            _organizationDocRepo = organizationDocRepo;
            _organizationMemberRepo = organizationMemberRepo;
            _cloudinaryService = cloudinaryService;
            _userRepo = userRepo;
        }
        public async Task<int> GetActiveOrganizationCountAsync()
        {
            return await _organizationRepo.GetActiveOrganizationCountAsync();
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

        public async Task<int> CreateOrganizationAsync(CreateOrganizationDto dto, Guid creatorUserId, string? logoUrl)
        {
            // 1️⃣ Check if user already has an active (non-rejected) organization
            if (await UserHasActiveOrganizationAsync(creatorUserId))
            {
                throw new InvalidOperationException("You already have an active organization (pending, approved, or blacklisted).");
            }

            // 2️⃣ Create Organization entity (without logo & PayQr yet)
            var organization = new Organization
            {
                Name = dto.Name,
                OrganizationEmail = dto.OrganizationEmail,
                Description = dto.Description,
                PhoneNumber = dto.PhoneNumber,
                Status = (int)Status.Pending,
                IsBlackListedOrg = false,
                Address = dto.Address
            };

            // 3️⃣ Handle Logo (upload or default)
            if (dto.Logo != null)
            {
                var logoUpload = await _cloudinaryService.UploadFileAsync(dto.Logo);
                organization.LogoUrl = logoUpload.SecureUrl;
            }
            else
            {
                organization.LogoUrl = logoUrl ?? "/images/default-logo.png"; // default logo
            }

            // 4️⃣ Handle PayQrUrl (upload if file provided)
            if (dto.PayQrUrls != null)
            {
                var qrUpload = await _cloudinaryService.UploadFileAsync(dto.PayQrUrls);
                organization.PayQrUrls = qrUpload.SecureUrl; // save uploaded QR code image
            }
            else
            {
                organization.PayQrUrls = null; // optional: could set to default QR image
            }

            // 5️⃣ Save Organization to get its Id
            await _organizationRepo.AddAsync(organization);
            await _organizationRepo.SaveChangesAsync();

            // 6️⃣ Upload split files (NRC front/back, Certificate)
            var splitFiles = new List<IFormFile?> { dto.NrcFront, dto.NrcBack, dto.Certificate };
            var splitFileLabels = new List<string> { "NRC Front", "NRC Back", "Certificate" };

            for (int i = 0; i < splitFiles.Count; i++)
            {
                var file = splitFiles[i];
                if (file == null) continue;

                var uploadResult = await _cloudinaryService.UploadFileAsync(file);

                var orgDoc = new OrganizationDoc
                {
                    OrganizationId = organization.Id,
                    ImageUrl = uploadResult.SecureUrl,
                    FileName = uploadResult.FileName ?? splitFileLabels[i],
                    FileType = uploadResult.FileType,
                    CreatedAt = DateTime.UtcNow
                };

                await _organizationDocRepo.AddAsync(orgDoc);
            }

            // 7️⃣ Upload any additional documents
            if (dto.Documents != null && dto.Documents.Count > 0)
            {
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
            }

            // 8️⃣ Save all OrganizationDocs
            await _organizationDocRepo.SaveChangesAsync();

            // 9️⃣ Add creator as Owner in OrganizationMembers
            var ownerMember = new OrganizationMember
            {
                OrganizationId = organization.Id,
                UserId = creatorUserId,
                RoleInOrg = "Owner",
                JoinedAt = DateTime.UtcNow,
                IsAccepted = true
            };
            await _organizationMemberRepo.AddAsync(ownerMember);
            await _organizationMemberRepo.SaveChangesAsync();

            // 🔟 Return the new Organization Id
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
                LogoUrl = org.LogoUrl, // ✅ Include logo
                Address = org.Address,
                PayQrUrls = org.PayQrUrls,
                IsBlackListedOrg = org.IsBlackListedOrg,
                Status = (Status)org.Status,
                ApprovedBy = org.ApprovedBy,
                ApprovedAt = org.ApprovedAt,
                CreatedAt = org.CreatedAt,
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
                LogoUrl = org.LogoUrl,
                Address = org.Address,
                PayQrUrls = org.PayQrUrls,
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
                LogoUrl = org.LogoUrl,
                Address= org.Address,
                PayQrUrls = org.PayQrUrls,
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
                LogoUrl = org.LogoUrl,
                Address = org.Address,
                PayQrUrls = org.PayQrUrls,
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
            string? approverName = null;
            if (org.ApprovedBy != null)
            {
                var approverUser = await _userRepo.GetUserByIdAsync(org.ApprovedBy.Value);
                if (approverUser != null)
                {
                    // Assume user has FullName property or fallback to Email
                    approverName = !string.IsNullOrEmpty(approverUser.Name)
                        ? approverUser.Name
                        : approverUser.Email;
                }
            }

            var dto = new OrganizationDto
            {
                Id = org.Id,
                Name = org.Name,
                OrganizationEmail = org.OrganizationEmail,
                Description = org.Description,
                PhoneNumber = org.PhoneNumber,
                LogoUrl = org.LogoUrl,
                Address = org.Address,
                PayQrUrls = org.PayQrUrls,
                IsBlackListedOrg = org.IsBlackListedOrg,
                Status = (Status)org.Status,
                ApprovedBy = org.ApprovedBy,
                ApprovedByName = approverName,
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
            org.Address = dto.Address;

            if (dto.Logo != null) // ✅ Allow updating logo
            {
                var logoUpload = await _cloudinaryService.UploadFileAsync(dto.Logo);
                org.LogoUrl = logoUpload.SecureUrl;
            }
            // ✅ Update Pay QR if new one is uploaded
            if (dto.PayQrUrls != null) // <-- assuming UpdateOrganizationDto has IFormFile PayQr
            {
                var qrUpload = await _cloudinaryService.UploadFileAsync(dto.PayQrUrls);
                org.PayQrUrls = qrUpload.SecureUrl;
            }

            _organizationRepo.Update(org);
            return await _organizationRepo.SaveChangesAsync();
        }

        //new
        public async Task<string?> UpdateLogoAsync(int orgId, IFormFile logoFile, Guid userId)
        {
            var org = await _organizationRepo.GetByIdAsync(orgId);
            if (org == null) return null;

            var uploadResult = await _cloudinaryService.UploadFileAsync(logoFile);
            org.LogoUrl = uploadResult.SecureUrl;

            _organizationRepo.Update(org);
            await _organizationRepo.SaveChangesAsync();

            return org.LogoUrl;
        } //

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
                    LogoUrl = org.LogoUrl,
                    Address = org.Address,
                    PayQrUrls = org.PayQrUrls,
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
