using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Models
{
    public class UserFilterOptions
    {
        public bool? OnlyBlacklisted { get; set; } = false;

        public int? RoleId { get; set; }
    }
}