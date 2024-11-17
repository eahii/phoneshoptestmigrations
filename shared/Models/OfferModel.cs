// File: Shared/Models/OfferModel.cs

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.Models
{
    public class OfferModel
    {
        [Key]
        public int OfferID { get; set; }

        [Required]
        [MaxLength(100)]
        public string PhoneBrand { get; set; }

        [Required]
        [MaxLength(100)]
        public string PhoneModel { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal OriginalPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int PhoneAge { get; set; }

        [Required]
        [Range(1, 100)]
        public int OverallCondition { get; set; }

        [Required]
        [Range(1, 100)]
        public int BatteryLife { get; set; }

        [Required]
        [Range(1, 100)]
        public int ScreenCondition { get; set; }

        [Required]
        [Range(1, 100)]
        public int CameraCondition { get; set; }

        public decimal? ResellValue { get; set; } // Added ResellValue

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending"; // Default value

        [Required]
        public DateTime SubmissionDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("UserID")]
        public virtual UserModel User { get; set; }

        public int UserID { get; set; }

        [ForeignKey("PhoneID")]
        public virtual PhoneModel Phone { get; set; } // Added Phone navigation property

        public int PhoneID { get; set; } // Added PhoneID foreign key
    }
}