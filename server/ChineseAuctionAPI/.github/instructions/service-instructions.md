Service Layer & Business Logic
•	Isolation: Business logic must reside in service classes, not controllers or repositories.
•	Naming: Service interfaces should be named I<Entity>Service and implementations as <Entity>Service (e.g., IDonorService, DonorService).
•	Dependency: Services depend on repository interfaces via constructor injection.
•	Async: Service methods should be async when calling repository methods.
•	Responsibility: Services orchestrate data access, validation, and business rules. Avoid direct database or HTTP context access.
•	Testing: Services should be unit-testable and expose interfaces for DI.