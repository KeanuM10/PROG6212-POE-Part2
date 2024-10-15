using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ClaimSystem.Models;
using System.Collections.Generic;
using System.Linq;
using System.IO;

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
        public IActionResult SubmitClaim(string lecturer, decimal hoursWorked, decimal hourlyRate, string notes, IFormFile document)
        {
            // Validate file upload
            string filePath = null;
            string originalFileName = null;

            notes = string.IsNullOrWhiteSpace(notes) ? "No additional notes" : notes;

            if (document != null && document.Length > 0)
            {
                // File size validation (limit: 5MB)
                if (document.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("document", "The file size cannot exceed 5MB.");
                }

                // File type validation
                var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                var extension = Path.GetExtension(document.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("document", "Only .pdf, .docx, and .xlsx files are allowed.");
                }

                // If validation errors - return to form and display errors
                if (!ModelState.IsValid)
                {
                    return View("ClaimSubmission");
                }

                // Save file to server (in wwwroot/uploads)
                var uploadsDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                var uniqueFileName = Path.GetRandomFileName() + extension;
                filePath = Path.Combine(uploadsDirectory, uniqueFileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    document.CopyTo(stream);
                }
                originalFileName = document.FileName;
            }

            // Create new claim based on submitted data
            var newClaim = new Claim
            {
                ClaimID = _claims.Count + 1,
                Lecturer = lecturer, // Lecturer can now input own name
                Hours = hoursWorked,
                HourlyRate = hourlyRate, // Hourly rate
                Notes = notes, // Notes added
                Status = "Pending",
                LastUpdated = System.DateTime.Now,
                SupportingDocumentPath = filePath,  // Store file path in claim
                OriginalFileName = originalFileName  // Store original file name
            };

            // Add new claim to list
            _claims.Add(newClaim);

            // Redirect to ClaimStatus action after a submission
            return RedirectToAction("ClaimStatus");
        }

        // GET: Login page
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login process
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Role login
            if (username == "coordinator" && password == "123")
            {
                HttpContext.Session.SetString("UserRole", "Coordinator");
                return RedirectToAction("ClaimApproval");
            }
            else if (username == "manager" && password == "123")
            {
                HttpContext.Session.SetString("UserRole", "Manager");
                return RedirectToAction("ClaimApproval");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View();
            }
        }

        // Logout action
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();  // Clear session
            return RedirectToAction("Login");
        }

        // ClaimApproval action - shows pending claims for approval / rejection
        public IActionResult ClaimApproval()
        {
            // Filter and give only pending claims
            if (HttpContext.Session.GetString("UserRole") == null)
            {
                return RedirectToAction("Login");
            }

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

        // Error action
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
