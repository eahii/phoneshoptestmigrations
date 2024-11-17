// File: Shared/Models/UserModel.cs

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
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(50)]
        public string Role { get; set; } = "Customer"; // Default role

        [Required]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        public string Address { get; set; }

        [Phone]
        [MaxLength(20)]
        public string PhoneNumber { get; set; }

        // Navigation Properties

        // One-to-One: UserModel <-> ShoppingCartModel
        public virtual ShoppingCartModel ShoppingCart { get; set; }

        // One-to-Many: UserModel -> OfferModel
        public virtual ICollection<OfferModel> Offers { get; set; } = new List<OfferModel>();

        // One-to-Many: UserModel -> OrderModel
        public virtual ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
    }
}