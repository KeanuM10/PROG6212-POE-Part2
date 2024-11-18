using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ClaimSystem.Models;
using System;
using System.IO;
using System.Linq;
using ClaimSystem.Data;

namespace ClaimSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Index action - displays welcome screen
        public IActionResult Index()
        {
            return View();
        }

        // ClaimSubmission action - displays claim submission form
        public IActionResult ClaimSubmission()
        {
            // Only allow access if the user is logged in as a Lecturer
            if (HttpContext.Session.GetString("UserRole") != "Lecturer")
            {
                return RedirectToAction("Login");
            }
            return View();
        }

        // SubmitClaim action - handles POST requests to submit claims
        [HttpPost]
        public IActionResult SubmitClaim(string lecturer, decimal hoursWorked, decimal hourlyRate, string notes, IFormFile document)
        {
            if (string.IsNullOrEmpty(lecturer))
            {
                ModelState.AddModelError("lecturer", "Lecturer name is required.");
            }

            if (hoursWorked <= 0)
            {
                ModelState.AddModelError("hoursWorked", "Hours worked must be greater than zero.");
            }

            if (hourlyRate <= 0)
            {
                ModelState.AddModelError("hourlyRate", "Hourly rate must be greater than zero.");
            }

            // Validate file upload
            string filePath = null;
            string originalFileName = null;

            if (document != null && document.Length > 0)
            {
                try
                {
                    var allowedExtensions = new[] { ".pdf", ".docx", ".xlsx" };
                    var extension = Path.GetExtension(document.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        ModelState.AddModelError("document", "Only .pdf, .docx, and .xlsx files are allowed.");
                    }

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
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unexpected error occurred while uploading the document.");
                    return View("ClaimSubmission");
                }
            }

            if (!ModelState.IsValid)
            {
                return View("ClaimSubmission");
            }

            // Save the claim to the database
            var newClaim = new Claim
            {
                Lecturer = lecturer,
                Hours = hoursWorked,
                HourlyRate = hourlyRate,
                Notes = string.IsNullOrWhiteSpace(notes) ? "No additional notes" : notes,
                Status = "Pending",
                LastUpdated = DateTime.Now,
                SupportingDocumentPath = filePath,
                OriginalFileName = originalFileName
            };

            _context.Claims.Add(newClaim);
            _context.SaveChanges();

            return RedirectToAction("ClaimStatus");
        }

        // ClaimStatus action - displays status of submitted claims
        public IActionResult ClaimStatus()
        {
            if (HttpContext.Session.GetString("UserRole") == null)
            {
                return RedirectToAction("Login");
            }

            var claims = _context.Claims.ToList();
            return View(claims);
        }

        // ClaimApproval action - shows pending claims for approval/rejection
        public IActionResult ClaimApproval()
        {
            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
            {
                return RedirectToAction("Login");
            }

            var claims = _context.Claims.Where(c => c.Status == "Pending").ToList();
            return View(claims);
        }

        // ApproveClaim action - approves a claim based on claim ID
        public IActionResult ApproveClaim(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                claim.Status = "Approved";
                claim.LastUpdated = DateTime.Now;
                _context.SaveChanges();
            }

            return RedirectToAction("ClaimApproval");
        }

        // RejectClaim action - rejects a claim based on claim ID
        public IActionResult RejectClaim(int id)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim != null)
            {
                claim.Status = "Rejected";
                claim.LastUpdated = DateTime.Now;
                _context.SaveChanges();
            }

            return RedirectToAction("ClaimApproval");
        }

        // Login action
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == "lecturer" && password == "123")
            {
                HttpContext.Session.SetString("UserRole", "Lecturer");
                return RedirectToAction("ClaimSubmission");
            }
            else if (username == "coordinator" && password == "123")
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

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
