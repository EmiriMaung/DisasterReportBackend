using System.Text.Json.Serialization;

namespace DisasterReport.Services.Models
{
    public class DisasterTopicDto
    {
        public int Id { get; set; }
        public string? TopicName { get; set; }
        public Guid? AdminId { get; set; }
    }

    public class CreateDisasterTopicDto
    {
        public string TopicName { get; set; } = string.Empty;
        [JsonIgnore]
        public Guid AdminId { get; set; }  // MUST have AdminId to track creator
    }

    public class UpdateDisasterTopicDto
    {
        public int Id { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public Guid AdminId { get; set; }            // original creator (optional)
        public Guid? UpdatedAdminId { get; set; }    // who updated (optional)
    }
}
