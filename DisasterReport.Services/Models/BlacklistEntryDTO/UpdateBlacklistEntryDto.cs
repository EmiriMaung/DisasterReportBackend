using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models.BlacklistEntryDTO
{
    public class UpdateBlacklistEntryDto
    {
        public string Reason { get; set; } = null!;

        public Guid UpdatedAdminId { get; set; }
    }
}
