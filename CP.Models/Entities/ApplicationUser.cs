using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CP.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(255)]
        public string? ProfileImage { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsOnline { get; set; } = false;

        public AvailabilityStatus AvailabilityStatus { get; set; } = AvailabilityStatus.Active;
            
        public DateTime? LastSeenTimestamp { get; set; }

        public virtual ICollection<Conversation> ConversationsAsUser1 { get; set; }
        public virtual ICollection<Conversation> ConversationsAsUser2 { get; set; }

        public virtual ICollection<FriendRequest> FriendRequestSender { get; set; }
        public virtual ICollection<FriendRequest> FriendRequestReceiver { get; set; }
    }

    public enum AvailabilityStatus
    {
        Active = 1,
        Away,
        DoNotDisturb,
        Invisible
    }
}
