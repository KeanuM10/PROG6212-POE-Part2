using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ClaimSystem.Models;
using System;
using System.IO;
using System.Linq;
using ClaimSystem.Data;
using iTextSharp.text;
using iTextSharp.text.pdf;
using BCrypt.Net;

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
        public IActionResult SubmitClaim(decimal hoursWorked, decimal hourlyRate, string notes, IFormFile document)
        {
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

            // Calculate total payment
            decimal totalPayment = Math.Round(hoursWorked * hourlyRate, 2); ;

            // Save the claim to the database
            var newClaim = new Claim
            {
                Hours = Math.Round(hoursWorked, 2),
                HourlyRate = Math.Round(hourlyRate, 2),
                TotalPayment = totalPayment,
                Notes = string.IsNullOrWhiteSpace(notes) ? "No additional notes" : notes,
                Status = "Pending",
                LastUpdated = DateTime.Now,
                SupportingDocumentPath = filePath,
                OriginalFileName = originalFileName
            };

            // Automated validation
            var validationResult = ValidateClaim(newClaim);

            if (validationResult.StartsWith("Rejected"))
            {
                newClaim.AutoStatus = "Auto Rejected";
                newClaim.Status = "Rejected"; // Set status to Rejected
            }
            else
            {
                newClaim.AutoStatus = "Auto Approved";
                newClaim.Status = "Approved"; // Set status to Approved
            }

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

            var claims = _context.Claims.ToList();
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
        // Fetch user from the database
        var user = _context.Users.FirstOrDefault(u => u.Username == username);

        if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
        {
            HttpContext.Session.SetString("UserRole", user.Role);
            if (user.Role == "Lecturer")
            {
                return RedirectToAction("ClaimSubmission");
            }
            else if (user.Role == "Manager" || user.Role == "Coordinator")
            {
                return RedirectToAction("ClaimApproval");
            }
        }

        ViewBag.ErrorMessage = "Invalid username or password.";
        return View();
    }


    private string ValidateClaim(Claim claim)
        {
            // Example rules
            if (claim.Hours > 80)
            {
                return "Rejected: Exceeds maximum hours allowed (80).";
            }

            if (claim.HourlyRate < 10 || claim.HourlyRate > 100)
            {
                return "Rejected: Hourly rate is out of the allowed range (R10-R100).";
            }

            if (claim.TotalPayment > 8000)
            {
                return "Rejected: Total payment exceeds the R8,000 threshold.";
            }

            return "Approved";
        }

        [HttpPost]
        public IActionResult OverrideDecision(int id, string action)
        {
            var claim = _context.Claims.FirstOrDefault(c => c.ClaimID == id);
            if (claim == null)
            {
                return NotFound();
            }

            var userRole = HttpContext.Session.GetString("UserRole");
            if (userRole != "Coordinator" && userRole != "Manager")
            {
                return Unauthorized();
            }

            // Update the claim status based on the action
            claim.OverriddenStatus = action == "Approve" ? "Approved" : "Rejected";
            claim.OverriddenBy = userRole;
            claim.Status = claim.OverriddenStatus; // Update claim status
            claim.LastUpdated = DateTime.Now;

            _context.SaveChanges();

            return RedirectToAction("ClaimApproval");
        }

        public IActionResult GenerateReport()
        {
            // Fetch approved claims and summary data
            var approvedClaims = _context.Claims
                .Where(c => c.Status == "Approved")
                .ToList();

            int totalClaims = _context.Claims.Count();
            int totalApproved = approvedClaims.Count;
            int totalRejected = _context.Claims.Count(c => c.Status == "Rejected");
            decimal totalApprovedPayments = approvedClaims.Sum(c => c.TotalPayment);
            decimal totalApprovedHours = approvedClaims.Sum(c => c.Hours);

            // Generate the PDF
            using (var stream = new MemoryStream())
            {
                var document = new Document();
                PdfWriter.GetInstance(document, stream);
                document.Open();

                // Title
                var titleFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 16);
                document.Add(new Paragraph("Approved Claims Report", titleFont));
                document.Add(new Paragraph($"Generated on: {DateTime.Now:dd MMM yyyy HH:mm}\n\n"));

                // Summary Section
                var summaryFont = FontFactory.GetFont(FontFactory.HELVETICA, 12);
                document.Add(new Paragraph($"Total Claims Submitted: {totalClaims}", summaryFont));
                document.Add(new Paragraph($"Total Claims Approved: {totalApproved}", summaryFont));
                document.Add(new Paragraph($"Total Claims Rejected: {totalRejected}", summaryFont));
                document.Add(new Paragraph($"Total Payment for Approved Claims: R{totalApprovedPayments:F2}", summaryFont));
                document.Add(new Paragraph($"Total Hours for Approved Claims: {totalApprovedHours:F2}\n\n", summaryFont));

                // Approved Claims Details Table
                var table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new[] { 10f, 20f, 10f, 10f, 20f, 30f }); // Adjust widths as needed

                // Table Headers
                var headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10);
                table.AddCell(new PdfPCell(new Phrase("Claim ID", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Lecturer", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Hours", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Total Payment", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Last Updated", headerFont)));
                table.AddCell(new PdfPCell(new Phrase("Notes", headerFont)));

                // Table Data
                var cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 9);
                foreach (var claim in approvedClaims)
                {
                    table.AddCell(new PdfPCell(new Phrase(claim.ClaimID.ToString(), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.Lecturer, cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.Hours.ToString("F2"), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase($"R{claim.TotalPayment:F2}", cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.LastUpdated.ToString("dd MMM yyyy"), cellFont)));
                    table.AddCell(new PdfPCell(new Phrase(claim.Notes, cellFont)));
                }

                // Add the table to the document
                document.Add(table);

                document.Close();

                return File(stream.ToArray(), "application/pdf", "ApprovedClaimsReport.pdf");
            }
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
