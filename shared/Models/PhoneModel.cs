// File: Shared/Models/PhoneModel.cs

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    public class PhoneModel
    {
        [Key]
        public int PhoneID { get; set; } // Primary Key

        [Required]
        [MaxLength(100)]
        public string Brand { get; set; }

        [Required]
        [MaxLength(100)]
        public string Model { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        [MaxLength(50)]
        public string Condition { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        // Navigation Properties
        public virtual ICollection<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
        public virtual ICollection<OrderItemModel> OrderItems { get; set; } = new List<OrderItemModel>();
        public virtual ICollection<OfferModel> Offers { get; set; } = new List<OfferModel>();
    }
}