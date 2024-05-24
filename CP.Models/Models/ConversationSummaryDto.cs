namespace CP.Models.Models
{
    public class ConversationSummaryDto
    {
        public int? ConversationId { get; set; }
        public int? FriendRequestId { get; set; }
        public int NumberOfUnseenMessages { get; set; }
        public ContactDto Contact { get; set; }
        public int ConversationType { get; set; }
        public MessageDto LastMessage { get; set; }
    }
}
