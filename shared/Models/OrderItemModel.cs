using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    public class OrderItemModel
    {
        [Key]
        public int OrderItemID { get; set; } // Primary Key

        [Required]
        public int OrderID { get; set; } // Foreign Key to Order

        [Required]
        public int PhoneID { get; set; } // Foreign Key to Phone

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; } // Price at the time of order

        // Navigation Properties
        [ForeignKey("OrderID")]
        public virtual OrderModel Order { get; set; }

        [ForeignKey("PhoneID")]
        public virtual PhoneModel Phone { get; set; }
    }
}