using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Enums
{
    public static class EnumHelper
    {
        public static string GetStatusName(int statusValue)
        {
            return Enum.IsDefined(typeof(Status), statusValue)
                ? ((Status)statusValue).ToString()
                : "Unknown";
        }
    }
}
