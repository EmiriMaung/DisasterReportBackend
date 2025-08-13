using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.BlacklistEntryDTO
{
    public class BlacklistExportDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public string Reason { get; set; }
        public string? UpdatedReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeletedFromBlacklist { get; set; }
        public string CreatedAdminName { get; set; }
        public string? UpdatedAdminName { get; set; }
    }
}
