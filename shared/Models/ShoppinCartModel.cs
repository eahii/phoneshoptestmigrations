using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    public class ShoppingCartModel
    {
        [Key]
        public int CartID { get; set; } // Primary Key

        [Required]
        public int UserID { get; set; } // Foreign Key to User

        // Navigation Properties

        [ForeignKey("UserID")]
        public virtual UserModel User { get; set; }

        public virtual ICollection<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
    }
}