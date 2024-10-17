#Overview

The Claim System allows lecturers to submit claims for the hours they've worked, including uploading supporting documents. Coordinators and managers can review, approve, or reject these claims. The system keeps track of the current status of each claim.

#Features:

Lecturer Claim Submission: Lecturers can submit their claims with hours worked and hourly rates.
File Upload: Optional file upload for supporting documents (PDF, DOCX, XLSX - Max 5MB).
Claim Approval/Reject: Coordinators and managers can view all pending claims and either approve or reject them (They are also able to view the uploaded documents both in the app, and in the saved file).
Status Tracking: Claim statuses are tracked (Pending, Approved, Rejected). These statuses are updated in real-time based on the manager / coordinators actions.
Role-based Access: Lecturers, coordinators, and managers have different roles within the application (Managers and coordinators have the same functionality) .

#Technologies Used:
ASP.NET Core MVC: Backend framework
Razor Views: Frontend rendering
Bootstrap 4: Responsive styling
XUnit: Unit testing framework
Moq: Mocking framework for unit tests

#Getting Started:
- Prerequisites:
- 
Before you begin, ensure you have the following installed:
.NET SDK
Visual Studio 2022
SQL Server or another relational database (For future reference, currently I am using in-memory data for simplicity).

#Running the Application:

Build and run the application by pressing F5 or selecting Start Debugging in Visual Studio 2022.
The application will launch in your default browser.

Use the following credentials for login:
Lecturer - 
Username: lecturer 
Password: 123
Coordinator - 
Username: coordinator 
password: 123
Manager -
username: manager
password: 123

#Running Unit Tests:

In Visual Studio, navigate to the Test Explorer by going to Test - Test Explorer.
Run all unit tests by selecting the Run All button.
The tests are written using the XUnit framework, (Moq used for mocking dependencies).

#How to Use:

Submitting a Claim (Lecturer) -
Login as a lecturer with the provided credentials.
Navigate to Submit Claim.
Fill in the form with the hours worked (Required), hourly rate (Required), and any additional notes (Optional).
Optionally, upload a supporting document (PDF, DOCX, XLSX).
Click Submit Claim.
You will be redirected to the Claim Status page to view the current status of your claim, as well as the other claims submitted by lecturers (Pending / Approved / Rejected).

Approving/Rejecting Claims (Coordinator/Manager) -
Login as a coordinator or manager with the provided credentials.
Navigate to Claim Approval.
View all pending claims, and either approve or reject them by clicking the corresponding buttons.

