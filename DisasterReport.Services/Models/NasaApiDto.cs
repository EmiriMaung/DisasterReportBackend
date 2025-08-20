using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models
{
    public class EonetEvent
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<Category> Categories { get; set; }
        public List<Source> Sources { get; set; }
        public List<Geometry> Geometry { get; set; }
    }

    public class Category
    {
        public string Id { get; set; }
        public string Title { get; set; }
    }

    public class Source
    {
        public string Id { get; set; }
        public string Url { get; set; }
    }

    public class Geometry
    {
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public List<double> Coordinates { get; set; }
    }

    public class EonetResponse
    {
        public List<EonetEvent> Events { get; set; }
    }
}
