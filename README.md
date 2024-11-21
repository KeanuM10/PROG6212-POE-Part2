#Contract Monthly Claim System

Overview:

The Contract Monthly Claim System is a web-based application designed to streamline the submission, tracking, and approval of monthly claims for contract-based employees, particularly lecturers. The system enables efficient management of claims, ensuring transparency, accountability, and ease of use for all stakeholders.

Features:

Core Features -

●	Lecturer Claim Submission:
Lecturers can submit claims with details such as hours worked, hourly rate, and additional notes.
  
●	File Upload:
Lecturers can upload optional supporting documents (PDF, DOCX, XLSX - Max 5MB).
  
●	Claim Approval/Rejection:
Admins can view pending claims, approve/reject them, and provide override reasons if necessary.
Supporting documents are accessible directly from the app.

●	Status Tracking:
Claims are categorised as Approved, or Rejected, with real-time updates based on admin actions.
Auto-approval or rejection based on predefined validation rules.

●	Override Reason Logging:
Admins can provide detailed reasons for overriding automated decisions, ensuring accountability.

●	User Management:
HR Managers can view, update, or delete user accounts, including changing roles and credentials.

●	PDF Reporting:
Generate professional, detailed PDF reports of approved claims for record-keeping or auditing.

Role-Based Access -
●	Lecturers: Submit claims, upload documents, and track claim statuses.
●	Admins: Approve/reject claims, provide override reasons, and generate reports.
●	HR Managers: Manage user accounts and roles with full access to user data, they can also generate extensive reports.

Technologies Used -
●	ASP.NET Core MVC: Backend framework for building scalable, secure applications.
●	Razor Views: Simplified and dynamic frontend rendering.
●	Bootstrap 4: For responsive and user-friendly styling.
●	iText: Creating of PDF reports
●	MySQL: Database management.

Getting Started:

Prerequisites -

Before running the application, ensure the following are installed:
●	.NET SDK
●	Visual Studio 2022
●	MySQL or another relational database
●	WampServer or any other server side validation software

Running the Application -
1.	Open the project in Visual Studio 2022.
2.	Open and run Wampserver or an equivalent.
3.	Open MySQL or another relation database.
4.	Create and import the necessary database tables and values with the provided SQL script.
5.	Build and run the application by pressing F5 or selecting Start Debugging.
6.	The application will launch in your default browser.

Login Credentials -

●	Lecturer:
Username: John
Password: lecturer1pass

●	Admin:
Username: Manager
Password: manpass

●	HR Manager:
Username: HR
Password: hrpassword

How to Use:

Submitting a Claim (Lecturer) -
1.	Log in as a Lecturer.
2.	Navigate to Submit Claim.
3.	Enter the required details:
○	Hours worked
○	Hourly rate
○	Additional notes
4.	Optionally, upload a supporting document (PDF, DOCX, XLSX).
5.	Click Submit Claim to save your claim.
6.	You will be redirected to Claim Status, where you can view the status of your claim.
7.	Claim Status will show approved or rejected based on predefined criteria, but admins may overrule this outcome.

Approving/Rejecting Claims (Admin) -
1.	Log in as an Admin.
2.	Navigate to Claim Approval.
3.	Review pending claims.
4.	Approve or reject claims.
5.	Optionally provide a reason for overriding the automated decision.

Managing Users (HR Manager) -
1.	Log in as an HR Manager.
2.	Navigate to HR Management.
3.	Update user details or delete accounts as needed.
4.	Save changes to reflect updates.

Generating Reports (Admin / HR) -
1.	Navigate to Claim Approval (Admin) or HR Management (HR).
2.	Click the Download PDF Report button to generate a detailed report of approved claims.

Future Enhancements:

The system is designed to be scalable, with the potential for:
●	Advanced reporting features.
●	Integration with external payroll or financial systems.
●	Automated notifications for claim status updates.

Troubleshooting:

The system should function correctly, if it does not, try troubleshooting:
●	Ensure the database works correctly, drop and recreate the database, tables and insert statements if necessary.
●	If the app does not load, try rebuild the system in Visual Studio 2022
●	Delete Migration files and redo “Add-Migration InitialCreate” if the database does not match the web-apps’ latest info.


