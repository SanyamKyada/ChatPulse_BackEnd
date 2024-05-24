using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CP.Models.Entities
{
    public class FriendRequestMessage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [ForeignKey("FriendRequest")]
        public int FriendRequestId { get; set; }
        [Required]
        public string Content { get; set; }
        [Required]
        public DateTime Timestamp { get; set; }

       // [ForeignKey("FriendRequestId")]
        public virtual FriendRequest FriendRequest { get; set; }
    }
}
