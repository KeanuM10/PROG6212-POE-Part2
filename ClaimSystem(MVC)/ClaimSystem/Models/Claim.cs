using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClaimSystem.Models
{
    public class Claim
    {
        [Key]
        public int ClaimID { get; set; }

        [Required]
        public decimal Hours { get; set; }

        [Required]
        public decimal HourlyRate { get; set; }

        public string? Notes { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public decimal TotalPayment { get; set; }

        public string? SupportingDocumentPath { get; set; }

        public string? OriginalFileName { get; set; }

        public string? AutoStatus { get; set; } // Auto Approved or Auto Rejected
        public string? OverriddenStatus { get; set; } // Approved or Rejected by admin
        public string? OverriddenBy { get; set; } // Admin who overrode the decision
        public string? ReasonForOverride { get; set; } // Reason provided by admin

    }


    [Table("User")]
    public class User
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; }

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } // Plain text password for simplicity

        [Required]
        public string Role { get; set; } // 'lecturer' or 'admin'
    }
}
