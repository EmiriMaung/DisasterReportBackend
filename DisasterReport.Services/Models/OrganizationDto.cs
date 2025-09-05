using DisasterReport.Services.Enums;
using Microsoft.AspNetCore.Http;

namespace DisasterReport.Services.Models
{
    public class OrganizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string OrganizationEmail { get; set; } = null!;
        public string? Description { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public bool IsBlackListedOrg { get; set; }
        public Status Status { get; set; }  
        public Guid? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; } 
        public DateTime? ApprovedAt { get; set; }
        public DateTime CreatedAt { get; set; } 

        public string? LogoUrl { get; set; }
        public string? Address { get; set; }
        public string? PayQrUrls { get; set; }

        public List<OrganizationDocDto>? Docs { get; set; } = new List<OrganizationDocDto>();
        public List<OrganizationMemberDto>? Members { get; set; } = new List<OrganizationMemberDto>();
    }

    public class CreateOrganizationDto
    {
        public string Name { get; set; } = null!;
        public string OrganizationEmail { get; set; } = null!;
        public string? Description { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public IFormFile? Logo { get; set; }
        public IFormFile? NrcFront { get; set; }   
        public IFormFile? NrcBack { get; set; }   
        public IFormFile? Certificate { get; set; } 
        public List<IFormFile> Documents { get; set; } = new(); //optional
        public string? Address { get; set; }
        public IFormFile? PayQrUrls { get; set; }
    }

    public class UpdateOrganizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string OrganizationEmail { get; set; } = null!;
        public string? Description { get; set; }
        public string PhoneNumber { get; set; } = null!;
        public IFormFile? Logo { get; set; }
        public string? Address { get; set; }
        public IFormFile? PayQrUrls { get; set; }
    }
}
