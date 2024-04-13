using System.ComponentModel.DataAnnotations;

namespace CP.Models.Entities
{
    public class RefreshToken
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public string? TokenId { get; set; }
        public string? Refresh_Token { get; set; }
        public bool? IsActive { get; set; }
    }
}
