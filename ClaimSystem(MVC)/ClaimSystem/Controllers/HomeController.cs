using Microsoft.AspNetCore.Mvc;
using ClaimSystem.Models;
using System.Collections.Generic;
using System.Linq;

namespace ClaimSystem.Controllers
{
    public class HomeController : Controller
    {
        // Static list of claims to acts as database storage
        private static List<Claim> _claims = new()
        {
            new Claim { ClaimID = 1, Lecturer = "John Doe", Hours = 20, HourlyRate = 100, Notes = "Prog class", Status = "Pending", LastUpdated = System.DateTime.Now },
            new Claim { ClaimID = 2, Lecturer = "Jane Smith", Hours = 15, HourlyRate = 150, Notes = "Tutoring services", Status = "Pending", LastUpdated = System.DateTime.Now }
        };

        // Index action - displays welcome screen
        public IActionResult Index()
        {
            return View();
        }

        // ClaimSubmission action - displays claim submission form
        public IActionResult ClaimSubmission()
        {
            return View();
        }

        // SubmitClaim action - handles POST requests to submit claims
        [HttpPost]
        public IActionResult SubmitClaim(string lecturer, decimal hoursWorked, decimal hourlyRate, string notes)
        {
            // Create new claim based on submitted data
            var newClaim = new Claim
            {
                ClaimID = _claims.Count + 1,
                Lecturer = lecturer, // Lecturer can now input own name
                Hours = hoursWorked,
                HourlyRate = hourlyRate, // Hourly rate
                Notes = notes, // Notes added
                Status = "Pending",
                LastUpdated = System.DateTime.Now
            };

            // Add new claim to list
            _claims.Add(newClaim);

            // Redirect to ClaimStatus action after a submission
            return RedirectToAction("ClaimStatus");
        }

        // ClaimApproval action - shows pending claims for approval / rejection
        public IActionResult ClaimApproval()
        {
            // Filter and give only pending claims
            return View(_claims.Where(c => c.Status == "Pending").ToList());
        }

        // ApproveClaim action - approves claim based on claim ID
        public IActionResult ApproveClaim(int id)
        {
            // Find claim by its ID
            var claim = _claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                // Update status - 'Approved'
                claim.Status = "Approved";
                claim.LastUpdated = System.DateTime.Now;
            }

            // Go back to ClaimApproval view
            return RedirectToAction("ClaimApproval");
        }

        // RejectClaim action - reject claim based on claim ID
        public IActionResult RejectClaim(int id)
        {
            // Find claim by ID
            var claim = _claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                // Update status to 'Rejected'
                claim.Status = "Rejected";
                claim.LastUpdated = System.DateTime.Now;
            }

            // Back to ClaimApproval view
            return RedirectToAction("ClaimApproval");
        }

        // ClaimStatus action - displays status of submitted claims
        public IActionResult ClaimStatus()
        {
            // Pass list of claims to view
            return View(_claims);
        }
    }
}
