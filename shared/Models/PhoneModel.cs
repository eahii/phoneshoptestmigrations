using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shared.Models
{
    public class PhoneModel
    {
        [Key]
        public int PhoneID { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public string Description { get; set; }

        [Required]
        public string Condition { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; }

        // Navigation Properties
        public virtual ICollection<CartItemModel> CartItems { get; set; }
        public virtual ICollection<OrderItemModel> OrderItems { get; set; }
        public virtual ICollection<OfferModel> Offers { get; set; }
    }
}