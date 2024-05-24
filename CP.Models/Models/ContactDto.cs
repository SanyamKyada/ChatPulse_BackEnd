namespace CP.Models.Models
{
    public class ContactDto
    {
        public string ContactId { get; set; }
        public string Name { get; set; }
        public string ProfileImage { get; set; }
        public bool IsOnline { get; set; }
        public DateTime? LastSeenTimestamp { get; set; }
        public bool IsFriend { get; set; }
    }
}
