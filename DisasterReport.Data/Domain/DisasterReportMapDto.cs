using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Data.Domain
{
    //public class DisasterReportMapDto
    //{
    //    public int Id { get; set; }
    //    public string? Title { get; set; }
    //    public string? Description { get; set; }
    //    public string? Category { get; set; }
    //    public DateTime ReportedAt { get; set; }
    //    public DateTime? UpdatedAt { get; set; }
    //    public int Status { get; set; }
    //    public bool IsUrgent { get; set; }
    //    public string? TopicName { get; set; }
    //    public string? TownshipName { get; set; }
    //    public string? RegionName { get; set; }
    //    public decimal? Latitude { get; set; }
    //    public decimal? Longitude { get; set; }
    //    public string? ImpactTypes { get; set; }
    //    public string? SupportTypes { get; set; }
    //    public string? PrimaryImageUrl { get; set; }
    //}
    public class DisasterReportMapDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public DateTime? ReportedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int Status { get; set; }
        public bool IsUrgent { get; set; }

        public string? TopicName { get; set; }
        public string? TownshipName { get; set; }
        public string? RegionName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        public string? ImpactTypes { get; set; }
        public string? SupportTypes { get; set; }
        public string? MediaUrls { get; set; }
    }



    public class ReportFilterDto
    {
        public int? TopicId { get; set; }
        public string? TownshipName { get; set; }
        public string? RegionName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool? IsUrgent { get; set; }
    }
    public class CategoryCountDto
    {
        public int TopicId { get; set; }
        public string TopicName { get; set; }
        public string Category { get; set; }
        public int ReportYear { get; set; }
        public int ReportMonth { get; set; }
        public int CategoryCount { get; set; }
    }

}
