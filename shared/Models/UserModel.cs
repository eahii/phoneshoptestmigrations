using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class UserModel
    {
        [Key] // This attribute indicates that this property is the primary key
        public int UserID { get; set; } // PK - Primary Key, käyttäjän yksilöllinen tunniste
        public string Email { get; set; }
        public string Password { get; set; } // For user input
        public string PasswordHash { get; set; } // For storage
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = "Customer"; // Käyttäjän rooli (Customer/Admin)
        public DateTime CreatedDate { get; set; }
    }
}
