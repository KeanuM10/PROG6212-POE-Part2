using System;

namespace ClaimSystem.Models
{
    public class Claim
    {
        // Unique ID for claims
        public int ClaimID { get; set; }

        // Lecturer name - submitting claim
        public string Lecturer { get; set; } = string.Empty;  // Initialize as empty - avoid null problems

        // Hours worked (int)
        public int Hours { get; set; }

        // Current claim status - (Pending, Approved, Rejected)
        public string Status { get; set; } = "Pending";  // Default - Pending

        // Timestamp for last update for claim
        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
