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
        public bool IsOnline { get; set; } = false;

        public DateTime? LastSeenTimestamp { get; set; }

        public virtual ICollection<Conversation> ConversationsAsUser1 { get; set; }
        public virtual ICollection<Conversation> ConversationsAsUser2 { get; set; }
    }
}
