using Microsoft.AspNetCore.Identity;

namespace CP.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? ProfileImage { get; set; }
        public string? Name { get; set; }
    }
}
