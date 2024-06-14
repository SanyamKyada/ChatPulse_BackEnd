namespace CP.Models.Models
{
    public class MessageDto
    {
        public int MessageId { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsFromCurrentUser { get; set; }
        public bool IsWave { get; set; } = false;
    }
}
