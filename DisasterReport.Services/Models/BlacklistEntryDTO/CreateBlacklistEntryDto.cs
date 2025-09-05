using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.BlacklistEntryDTO
{
    public class CreateBlacklistEntryDto
    {
        public Guid UserId { get; set; }

        public string Reason { get; set; } = null!;

        public Guid CreatedAdminId { get; set; }

        public int ReportId { get; set; } 
    }
}
