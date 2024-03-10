using System;
using System.ComponentModel.DataAnnotations;

namespace CP.Models.Models
{
    public class RegistrationModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
