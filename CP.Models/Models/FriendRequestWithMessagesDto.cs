using CP.Models.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CP.Models.Models
{

    public class FriendRequestWithMessagesDto
    {
        public int Id { get; set; }
        public string SenderUserId { get; set; }
        public string ReceiverUserId { get; set; }
        public FriendRequestStatus Status { get; set; }
        public DateTime RequestTimeStamp { get; set; }
        public bool HasWaved {  get; set; }
        public List<FriendRequestMessagesDto> FriendRequestMessages { get; set;}
    }

    public class FriendRequestMessagesDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
