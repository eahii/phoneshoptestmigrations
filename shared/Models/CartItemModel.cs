// File: Shared/Models/CartItemModel.cs

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    public class CartItemModel
    {
        [Key]
        public int CartItemID { get; set; } // Primary Key

        [Required]
        public int CartID { get; set; } // Foreign Key to ShoppingCart

        [Required]
        public int PhoneID { get; set; } // Foreign Key to Phone

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        // Navigation Properties

        [ForeignKey("CartID")]
        public virtual ShoppingCartModel ShoppingCart { get; set; }

        [ForeignKey("PhoneID")]
        public virtual PhoneModel Phone { get; set; }
    }

    // Optional DTO for frontend
    public class CartItemWithPhone
    {
        public int CartItemID { get; set; }
        public int CartID { get; set; }
        public int Quantity { get; set; }
        public PhoneModel Phone { get; set; }
    }
}