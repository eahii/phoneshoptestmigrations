// File: Shared/Models/OrderModel.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    public class OrderModel
    {
        [Key]
        public int OrderID { get; set; } // Primary Key

        [Required]
        public int UserID { get; set; } // Foreign Key to User

        [Required]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal TotalPrice { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } // e.g., "Pending", "Completed", "Cancelled"

        // Navigation Properties

        [ForeignKey("UserID")]
        public virtual UserModel User { get; set; }

        public virtual ICollection<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();
    }
}