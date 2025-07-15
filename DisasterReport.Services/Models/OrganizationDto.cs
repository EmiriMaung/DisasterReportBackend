using DisasterReport.Services.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Status Status { get; set; }  // use an enum 
        public Guid? ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }

        public List<OrganizationDocDto>? Docs { get; set; } = new List<OrganizationDocDto>();
        public List<OrganizationMemberDto>? Members { get; set; } = new List<OrganizationMemberDto>();
    }

    public class CreateOrganizationDto
    {
        public string Name { get; set; } = null!;
        public string OrganizationEmail { get; set; } = null!;
        public string? Description { get; set; }
        public string PhoneNumber { get; set; } = null!;

        // For file uploads (e.g., documents to verify the org)
        public List<IFormFile> Documents { get; set; } = new();
    }

    public class UpdateOrganizationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string OrganizationEmail { get; set; } = null!;
        public string? Description { get; set; }
        public string PhoneNumber { get; set; } = null!;
    }
}
