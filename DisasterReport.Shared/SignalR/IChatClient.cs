namespace DisasterReport.Shared.SignalR
{
    public interface IChatClient
    {
        Task ReceiveDirectMessage(ChatMessageDto message);
        Task ReceiveTyping(TypingDto typing);
        Task ReceiveReadReceipt(ReadReceiptDto receipt);
    }

    public class ChatMessageDto
    {
        public string Id { get; set; } = Guid.NewGuid().ToString(); // temp
        public string FromUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime SentAtUtc { get; set; } = DateTime.UtcNow;
    }

    public class TypingDto
    {
        public string FromUserId { get; set; } = string.Empty;
        public string ToUserId { get; set; } = string.Empty;
        public bool IsTyping { get; set; }
    }

    public class ReadReceiptDto
    {
        public string MessageId { get; set; } = string.Empty;
        public string ReaderUserId { get; set; } = string.Empty;
        public DateTime ReadAtUtc { get; set; } = DateTime.UtcNow;
    }
}
