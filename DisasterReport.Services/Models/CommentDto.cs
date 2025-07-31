using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisasterReport.Services.Models
{
    public class CommentDto
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int DisasterReportId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public class CreateCommentDto
    {
        public int DisasterReportId { get; set; }
        public string? Content { get; set; }
    }

}
