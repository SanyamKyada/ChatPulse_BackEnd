using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CP.Models.Entities
{
    public class Conversation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string User1Id { get; set; }
        [ForeignKey("User1Id")]
        public virtual ApplicationUser User1 { get; set; }

        public DateTime User1LastSeen { get; set; }

        [Required]
        public string User2Id { get; set; }
        [ForeignKey("User2Id")]
        public virtual ApplicationUser User2 { get; set; }

        public DateTime User2LastSeen { get; set; }

        public virtual ICollection<Message> Messages { get; set; }
    }
}
