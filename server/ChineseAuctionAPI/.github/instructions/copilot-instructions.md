High-Level Project Overview
	•	Summary: ChineseAuctionAPI is a .NET 8 backend API for managing Chinese auction events, including authentication, item management, bidding, and reporting.
	•	Tech Stack: C#, ASP.NET Core (.NET 8), Entity Framework Core (SQL Server), JWT Authentication, Serilog, Swagger, BCrypt.Net-Next.
	•	Folder Structure:
		•	Controllers/ – API endpoints
		•	Services/ – Business logic
		•	Repositories/ – Data access
		•	Models/ – Entities and DTOs
		•	Data/ – DbContext and migrations
		•	Middleware/ – Custom middleware
		•	.github/ – Workflows and instructions
		•	appsettings.json – Configuration
		•	Program.cs – Entry point
    •	    Coding Principles:
		•	Use Repository Pattern for data access.
		•	Isolate business logic in services.
		•	Keep controllers thin and focused on HTTP concerns.
		•	Secure endpoints with JWT and [Authorize].
		•	Use async/await throughout.
		•	Log with Serilog.
		•	Document with Swagger.
		•	Mark technical debt with TODO or HACK.