using System.ComponentModel.DataAnnotations;

namespace ClaimSystem.Models
{
    public class Claim
    {
        [Key]
        public int ClaimID { get; set; }

        [Required]
        public string Lecturer { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Hours worked must be greater than zero.")]
        public decimal Hours { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Hourly rate must be greater than zero.")]
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
}
