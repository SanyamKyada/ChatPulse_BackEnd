using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CP.Models.Entities
{
    public class FriendRequest
    {
        [Key]
        public int Id { get; set; }
        public string SenderUserId { get; set; }
        [ForeignKey("SenderUserId")]
        public virtual ApplicationUser SenderUser { get; set; }
        public string ReceiverUserId { get; set; }
        [ForeignKey("ReceiverUserId")]
        public virtual ApplicationUser ReceiverUser { get; set; }
        public FriendRequestStatus Status { get; set; }
        public DateTime RequestTimeStamp { get; set; }
    }

    public enum FriendRequestStatus
    {
        Pending,
        Accepted,
        Rejected
    }
}
