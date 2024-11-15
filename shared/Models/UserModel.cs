using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    public class UserModel
    {
        [Key]
        public int UserID { get; set; } // Primary Key

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [NotMapped]
        public string Password { get; set; } // For user input (not stored in DB)

        [Required]
        public string PasswordHash { get; set; } // Hashed password for storage

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? Address { get; set; }

        public string? PhoneNumber { get; set; }

        [Required]
        public string Role { get; set; } = "Customer"; // User role (Customer/Admin)

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual ShoppingCartModel ShoppingCart { get; set; }
        public virtual ICollection<CartItemModel> CartItems { get; set; }
        public virtual ICollection<OfferModel> Offers { get; set; }
        public virtual ICollection<OrderModel> Orders { get; set; }
    }
}