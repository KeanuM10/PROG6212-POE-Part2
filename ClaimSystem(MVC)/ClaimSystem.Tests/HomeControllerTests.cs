using Xunit;
using ClaimSystem.Controllers;
using ClaimSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ClaimSystem.Tests
{
    public class HomeControllerTests
    {
        [Fact]
        public void Index_Returns_ViewResult()
        {
            // Arrange
            var controller = new HomeController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void Login_InvalidCredentials_Returns_ViewWithErrorMessage()
        {
            // Arrange
            var controller = new HomeController();
            string username = "wrongUser";
            string password = "wrongPassword";

            var mockSession = new Mock<ISession>();
            var httpContext = new DefaultHttpContext { Session = mockSession.Object };
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = controller.Login(username, password) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Invalid username or password.", result.ViewData["ErrorMessage"]);
        }

        [Fact]
        public void Login_ValidCoordinatorCredentials_RedirectsTo_ClaimApproval()
        {
            // Arrange
            var controller = new HomeController();
            string username = "coordinator";
            string password = "123";

            var mockSession = new Mock<ISession>();
            var httpContext = new DefaultHttpContext { Session = mockSession.Object };
            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = controller.Login(username, password) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ClaimApproval", result.ActionName);
        }


        [Fact]
        public void SubmitClaim_Adds_NewClaim_And_RedirectsTo_ClaimStatus()
        {
            // Arrange
            var controller = new HomeController();
            string lecturer = "Test Lecturer";
            decimal hoursWorked = 5;
            decimal hourlyRate = 50;
            string notes = "Test Notes";

            // Mock file upload
            var content = "This is a test file.";
            var fileName = "test.pdf";
            var fileMock = new Mock<IFormFile>();
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.OpenReadStream()).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.Length).Returns(ms.Length);
            fileMock.Setup(_ => _.ContentType).Returns("application/pdf");

            var document = fileMock.Object;

            // Act
            var result = controller.SubmitClaim(lecturer, hoursWorked, hourlyRate, notes, document) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ClaimStatus", result.ActionName);

            // Verify that claim was added
            var addedClaim = HomeController._claims.FirstOrDefault(c => c.Lecturer == lecturer && c.Hours == hoursWorked);
            Assert.NotNull(addedClaim);
            Assert.Equal("Pending", addedClaim.Status);
        }

        [Fact]
        public void ApproveClaim_ValidClaimId_UpdatesClaimStatusToApproved()
        {
            // Arrange
            var controller = new HomeController();
            int claimId = 1;

            // Act
            var result = controller.ApproveClaim(claimId) as RedirectToActionResult;

            // Assert
            var claim = HomeController._claims.FirstOrDefault(c => c.ClaimID == claimId);
            Assert.NotNull(claim);
            Assert.Equal("Approved", claim.Status);
            Assert.Equal("ClaimApproval", result.ActionName);
        }

        [Fact]
        public void RejectClaim_ValidClaimId_UpdatesClaimStatusToRejected()
        {
            // Arrange
            var controller = new HomeController();
            int claimId = 2;

            // Act
            var result = controller.RejectClaim(claimId) as RedirectToActionResult;

            // Assert
            var claim = HomeController._claims.FirstOrDefault(c => c.ClaimID == claimId);
            Assert.NotNull(claim);
            Assert.Equal("Rejected", claim.Status);
            Assert.Equal("ClaimApproval", result.ActionName);
        }

        [Fact]
        public void ClaimStatus_Returns_ViewWithClaims()
        {
            // Arrange
            var controller = new HomeController();

            // Mock session
            var mockSession = new Mock<ISession>();
            var httpContext = new DefaultHttpContext { Session = mockSession.Object };

            var key = "UserRole";
            var value = System.Text.Encoding.UTF8.GetBytes("Lecturer");
            mockSession.Setup(s => s.TryGetValue(key, out value)).Returns(true);

            controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

            // Act
            var result = controller.ClaimStatus() as ViewResult;
            var model = result?.Model as List<Claim>;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<Claim>>(model);
            Assert.Equal(HomeController._claims.Count, model.Count);
        }


    }
}
