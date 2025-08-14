using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.BlacklistEntryDTO
{
    public class BlacklistEntryDto
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        public string UserEmail { get; set; } = null!;
        
        public string? UserName { get; set; }      
        
        public string? ProfilePictureUrl { get; set; }

        public Guid? CreatedAdminId { get; set; }

        public string? CreatedAdminName { get; set; }

        public string? Reason { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }

        public Guid? UpdatedAdminId { get; set; }

        public string? UpdatedAdminName { get; set; }

        public string? UnblockReason { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public bool IsDeletedFromBlacklist { get; set;  } = false;
    }
}
