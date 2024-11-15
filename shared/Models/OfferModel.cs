using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    public class OfferModel
    {
        [Key]
        public int OfferID { get; set; } // Primary Key

        [Required]
        public int UserID { get; set; } // Foreign Key to User

        [Required]
        public string PhoneBrand { get; set; }

        [Required]
        public string PhoneModel { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal OriginalPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int PhoneAge { get; set; } // Age in years

        [Required]
        [Range(0, 100)]
        public int OverallCondition { get; set; } // Percentage

        [Required]
        [Range(0, 100)]
        public int BatteryLife { get; set; } // Percentage

        [Required]
        [Range(0, 100)]
        public int ScreenCondition { get; set; } // Percentage

        [Required]
        [Range(0, 100)]
        public int CameraCondition { get; set; } // Percentage

        [Required]
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"

        [Required]
        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserID")]
        public virtual UserModel User { get; set; }
    }
}