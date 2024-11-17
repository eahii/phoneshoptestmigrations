using System.ComponentModel.DataAnnotations;

namespace Shared.DTOs
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [MaxLength(255)]
        public string Address { get; set; }

        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }
    }
}