using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models
{
    public class LocationDto
    {
        public int? Id { get; set; }
        public string TownshipName { get; set; } = null!;
        public string RegionName { get; set; } = null!;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }

}
