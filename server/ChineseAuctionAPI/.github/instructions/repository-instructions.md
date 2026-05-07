-Pattern: All data access must use the Repository Pattern. Repositories
encapsulate database operations and expose methods for querying and 
persisting entities.
-Async: Implement all data access methods using async/await (Task<T>, $Task),
leveraging Entity Framework Core's async APIs.
-Naming: Repository interfaces should be named I<Entity>Repository and
implementations as <Entity>Repository (e.g., IDonorRepository,
$DonorRepository).
-DbContext: Repositories interact with the DbContext; controllers and services
must not access DbContext directly.
-Separation: Each repository should handle a single entity or aggregate root.
-Testing: Expose interfaces for repositories to support dependency injection
and unit testing.