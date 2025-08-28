using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models
{
    public class UsgsEvent
    {
        public string Id { get; set; }
        public Properties Properties { get; set; }
        public Properties Title { get; set; }
        public Geometry Geometry { get; set; }
    }

    public class Properties
    {
        public double Mag { get; set; }
        public string Place { get; set; }
        public long Time { get; set; }
        public string Url { get; set; }
    }

    public class UsgsResponse
    {
        public List<UsgsEvent> Features { get; set; }
        public Metadata Metadata { get; set; }
    }

    public class Metadata
    {
        public int Count { get; set; }
        public long Generated { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
    }
}
